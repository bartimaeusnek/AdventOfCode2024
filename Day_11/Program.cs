namespace Day_11;

using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

class Program
{
    public static List<Stone> ParseInput(string input) => input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                                                .Select(x => new Stone(ulong.Parse(x)))
                                                                .ToList(); 
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    
    static async Task Main(string[] args)
    {
        Stone.Stones = new Dictionary<Stone, ulong>();
        
        foreach (var stone in ParseInput(await ReadInputAsync()))
        {
            Stone.Stones.Add(stone, 1ul);
        }
        ulong sum;
        for (int i = 0; i < 75; i++)
        {
            var arr = Stone.Stones.ToArray();
            Stone.Stones.Clear();
            for (int index = 0; index < arr.Length; index++)
            {
                var stonesKey = arr[index];
                stonesKey.Key.Blink(stonesKey.Value);
            }
            if (i == 24)
            {
                sum = Stone.Stones.Aggregate<KeyValuePair<Stone, ulong>, ulong>(0, (current, x) => current + x.Value);
                Console.WriteLine("Part 1: " + sum);
            }
        }
        sum = Stone.Stones.Aggregate<KeyValuePair<Stone, ulong>, ulong>(0, (current, x) => current + x.Value);
        Console.WriteLine("Part 2: " + sum);
    }

    public class Stone(ulong value) : IEquatable<Stone>
    {
        public override string ToString()
        {
            return _value.ToString();
        }
        public static Dictionary<Stone, ulong> Stones;
        private ulong _value = value;
        public ulong Value => _value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(int value)
        {
            return value % 2 == 0;
        }
        private static readonly ulong[] PowersOf10 =
        [
            1ul, 10ul, 100ul, 1000ul, 10000ul, 100000ul, 1000000ul, 10000000ul,
            100000000ul, 1000000000ul, 10000000000ul, 100000000000ul, 1000000000000ul,
            10000000000000ul, 100000000000000ul, 1000000000000000ul, 10000000000000000ul,
            100000000000000000ul, 1000000000000000000ul, 10000000000000000000ul
        ];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong Pow10(int value)
        {
            return PowersOf10[value];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log10(ulong i)
        {
            return (int)Lzcnt.X64.LeadingZeroCount(i) switch
            {
                64 or 63 or 62 or 61 => 0,
                60 => i < 10 ? 0 : 1,
                59 or 58 => 1,
                57 => i < 100 ? 1 : 2,
                56 or 55 => 2,
                54 => i < 1000 ? 2 : 3,
                53 or 52 or 51 => 3,
                50 => i < 10000 ? 3 : 4,
                49 or 48 => 4,
                47 => i < 100000 ? 4 : 5,
                46 or 45 => 5,
                44 => i < 1000000 ? 5 : 6,
                43 or 42 or 41 => 6,
                40 => i < 10000000 ? 6 : 7,
                39 or 38 => 7,
                37 => i < 100000000 ? 7 : 8,
                36 or 35 => 8,
                34 => i < 1000000000 ? 8 : 9,
                33 or 32 or 31 => 9,
                30 => i < 10000000000 ? 9 : 10,
                29 or 28 => 10,
                27 => i < 100000000000 ? 10 : 11,
                26 or 25 => 11,
                24 => i < 1000000000000 ? 11 : 12,
                23 or 22 or 21 => 12,
                20 => i < 10000000000000 ? 12 : 13,
                19 or 18 => 13,
                17 => i < 100000000000000 ? 13 : 14,
                16 or 15 => 14,
                14 => i < 1000000000000000 ? 14 : 15,
                13 or 12 or 11 => 15,
                10 => i < 10000000000000000 ? 15 : 16,
                9 or 8 => 16,
                7 => i < 100000000000000000 ? 16 : 17,
                6 or 5 => 17,
                4 => i < 1000000000000000000 ? 17 : 18,
                3 or 2 or 1 => 18,
                0 => i < 10000000000000000000UL ? 18 : 19,
                _ => throw new ArgumentException("Unexpected number of leading zeros")
            };
        }

        public void Blink(ulong count)
        {
            var digitsCheck = Log10(_value);
            Stone newStone;
            if (_value == 0)
            {
                newStone = new Stone(1);
            }
            else if (IsEven(digitsCheck + 1))
            {
                int digits = digitsCheck / 2 + 1;
                ulong divisor = Pow10(digits);
        
                ulong firstHalf = _value / divisor;
                ulong remainder = _value % divisor;

                newStone = new Stone(firstHalf);
                if (!Stones.TryAdd(newStone, count))
                {
                    Stones[newStone] = Stones[newStone] += count;
                }
                newStone = new Stone(remainder);
            }
            else
            {
                newStone = new Stone(_value * 2024ul);
            }
            if (!Stones.TryAdd(newStone, count))
            {
                Stones[newStone] = Stones[newStone] += count;
            }
        }
        
        public bool Equals(Stone other)
        {
            return _value == other._value;
        }
        public override bool Equals(object? obj)
        {
            return obj is Stone other && Equals(other);
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
        public static bool operator ==(Stone left, Stone right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Stone left, Stone right)
        {
            return !left.Equals(right);
        }
    }
}