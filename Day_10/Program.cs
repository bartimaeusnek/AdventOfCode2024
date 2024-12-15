// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_10;

class Program
{
    private const string SimpleDebugInput = """
                                      ...0...
                                      ...1...
                                      ...2...
                                      6543456
                                      7.....7
                                      8.....8
                                      9.....9
                                      """;

    private const string DebugInput = """
                                      89010123
                                      78121874
                                      87430965
                                      96549874
                                      45678903
                                      32019012
                                      01329801
                                      10456732
                                      """;

    public static void ParseInput(string input)
    {
        var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var ret = new Cell[lines.Length][];
        for (int y = 0; y < lines.Length; y++)
        {
            ret[y] = new Cell[lines[y].Length];
            for (int x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '.')
                {
                    ret[y][x] = new Cell(-1)
                    {
                        X = x,
                        Y = y
                    };
                    continue;
                }
                ret[y][x] = new Cell(lines[y][x] - '0')
                {
                    X = x,
                    Y = y
                };
            }
        }
        Cell.Field = ret;
    }
    
    public static void PrintField(Cell[][] field)
    {
        for (var y = 0; y < field.Length; y++)
        {
            for (int x = 0; x < field[y].Length; x++)
            {
                Console.Write(field[y][x].Value);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    
    static async Task Main(string[] args)
    {
        ParseInput(await ReadInputAsync());
        
        var fieldStartPointsFlattened = Cell.Field.SelectMany(x => x)
                                             .Where(x => x.Value == 0)
                                             .ToArray();

        ulong totalScore = 0;
        ulong totalRating = 0;
        Parallel.ForEach(fieldStartPointsFlattened, cell =>
        {
            var node = new AoCNode(cell.Value, cell.X, cell.Y);
            node.CheckNeighbours();
            Interlocked.Add(ref totalScore, (ulong) node.GetScore());
            Interlocked.Add(ref totalRating, (ulong) node.GetRating());
        });
        
        PrintField(Cell.Field);
        Console.WriteLine(totalScore);
        Console.WriteLine(totalRating);
        
        Thread.Sleep(-1);
    }
}
public class Cell(int value)
{
    public static Cell[][] Field = null!;
    public int X { get; set; }
    public int Y { get; set; }
    public readonly int Value = value;
}


public class AoCNode(int value, int x, int y) : IEquatable<AoCNode>
{
    public bool Equals(AoCNode? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Value == other.Value && X == other.X && Y == other.Y;
    }
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((AoCNode)obj);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, X, Y);
    }
    public static bool operator ==(AoCNode? left, AoCNode? right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(AoCNode? left, AoCNode? right)
    {
        return !Equals(left, right);
    }
    
    public readonly int Value = value;
    public readonly int X = x;
    public readonly int Y = y;
    public readonly List<AoCNode> Children = new List<AoCNode>();

    //Part2
    public int GetRating()
    {
        return Children.FirstOrDefault() is { Value: 9 } ? Children.Count : Children.Sum(aoCNode => aoCNode.GetRating());
    }

    //Part1
    public int GetScore()
    {
        return GetFlattenedChildren(Children).Where(x => x.Value == 9).Distinct().Count();
    }
    
    public static IEnumerable<AoCNode> GetFlattenedChildren(IEnumerable<AoCNode> input)
    {
        if (input.Any(x => x.Value == 9))
        {
            return input;
        }
        return input.SelectMany(x => GetFlattenedChildren(x.Children));
    }
    
    public void CheckNeighbours()
    {
        for (int y = -1; y <= 1; y++)
        {
            var dy = y + Y;
            if (dy >= Cell.Field.Length  || dy < 0)
            {
                continue;
            }
            for (int x = -1; x <= 1; x++)
            {
                if (Math.Abs(x) == Math.Abs(y))
                    continue;
                
                var dx = x + X;
                if (dx >= Cell.Field[dy].Length || dx < 0)
                {
                    continue;
                }
                var neighbour = Cell.Field[dy][dx];
                if (neighbour.Value != Value + 1)
                    continue;
                
                var nuChild = new AoCNode(neighbour.Value, dx, dy);
                Children.Add(nuChild);
                nuChild.CheckNeighbours();
            }
        }
    }
}