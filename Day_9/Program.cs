namespace Day_9;

using System.Text;

internal class Program
{
    private const string DebugInput = "2333133121414131402";
    private static string Remap(string input)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < input.Length; i++)
        {
            if (i % 2 == 0)
            {
                for (int j = 0; j < int.Parse(input[i].ToString()); j++)
                {
                    sb.Append((char) (i / 2));
                }
            }
            else
            {
                for (int j = 0; j < int.Parse(input[i].ToString()); j++)
                {
                    sb.Append(char.MaxValue);
                }
            }
        }
        return sb.ToString();
    }
    
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    
    public static async Task Part1()
    {
        var remapped = Remap(await ReadInputAsync());
        var done = false;
        var lastLength = remapped.Length;
        var sb = new StringBuilder(remapped.Length -1);
        do
        {
            lastLength = remapped.Length;
            var tmp = remapped[^1];
            
            var moved = false;
            for (int j = 0; j < remapped.Length - 1; j++)
            {
                if (remapped[j] != char.MaxValue || moved)
                    sb.Append(remapped[j]);
                else
                {
                    sb.Append(tmp);
                    moved = true;
                }
            }
            remapped = sb.ToString();
            if (!remapped.Contains(char.MaxValue))
                break;
            sb.Clear();
        } while (lastLength != remapped.Length);

        ulong checksum = 0;
        for (int i = 0; i < remapped.Length; i++)
        {
            checksum += remapped[i] * (ulong)i;
        }
        
        Console.WriteLine(checksum);
    }
    
    private static async Task Main(string[] args)
    {
        await Part1();
        await Part2();
    }

    private static async Task Part2()
    {
        var files = GetAmphipodFiles(await ReadInputAsync()).ToList();

        for (var i = files.Count - 1; i >= 0; i--)
        {
            if (files[i].IsFreeSpace)
                continue;
            for (int j = 0; j < i; j++)
            {
                if (!files[j].IsFreeSpace || files[j].Length < files[i].Length)
                    continue;
                
                var tmp = new AmphipodBlock(-1, files[i].Position, files[i].Length, true);
                files[i].Position = files[j].Position;
                files[j].Length -= files[i].Length;
                if (files[j].Length == 0)
                {
                    files.Remove(files[j]);
                }
                else
                {
                    files[j].Position = files[i].Position + files[i].Length;
                }
                files.Add(tmp);
                files = files.OrderBy(x => x.Position).ToList();
                break;
            }
        }
        ulong checksum = 0;
        var offset = 0;
        
        foreach (var currentFile in files)
        {
            if (!currentFile.IsFreeSpace)
            {
                for (int j = 0; j < currentFile.Length; j++)
                {
                    var tmp = (ulong)currentFile.Id * (ulong)offset;
#if DEBUG
                    Console.WriteLine($"{(ulong)currentFile.Id} * {(ulong)offset} = " +tmp);
#endif
                    checksum += tmp;
                    offset++;
                }
            }
            else
            {
                for (int j = 0; j < currentFile.Length; j++)
                {
#if DEBUG
                    Console.WriteLine($"0 * {(ulong)offset} = 0");
#endif
                    ++offset;
                }
            }
        }
        Console.WriteLine(checksum);
    }
    
    private static IEnumerable<AmphipodBlock> GetAmphipodFiles(string input)
    {
        int totalLength = 0;
        for (int i = 0; i < input.Length; i++)
        {
            var length = int.Parse(input[i].ToString());
            yield return new AmphipodBlock(i / 2, totalLength, length, i % 2 != 0);
            totalLength += length;
        }
    }
    
    private class AmphipodBlock(int id, int position, int length, bool isFreeSpace)
    {
        public int Id = id;
        public int Length = length;
        public bool IsFreeSpace = isFreeSpace;
        public int Position = position;
    }
}