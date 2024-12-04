// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_4;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

class Program
{
#if DEBUG
    private static string ReadInput()
    {
        return """
               MMMSXXMASM
               MSAMXMSMSA
               AMXSXMAAMM
               MSAMASMSMX
               XMASAMXAMM
               XXAMMXXAMA
               SMSMSASXSS
               SAXAMASAAA
               MAMMMXMMMM
               MXMXAXMASX
               """;
    }
#else
    private static string ReadInput()
    {
        using var reader = new StreamReader("input");
        return reader.ReadToEnd();
    }
#endif
    

    private static void PartTwo()
    {
        var rawinput = ReadInput();
        var input = rawinput.Split("\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.TrimEnd('\r'))
                            .ToArray();
#if DEBUG
        var debugOutput = new char[input.Length, input[0].Length];
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = 0; j < input[0].Length; j++)
            {
                debugOutput[i, j] = '.';
            }
        }
#endif
        var samsam = new XMas('S', 'S', 'M', 'M');
        var masmas = new XMas('M', 'M', 'S', 'S');
        var massam = new XMas('M', 'S', 'M', 'S');
        var sammas = new XMas('S', 'M', 'S', 'M');
        var count = 0;
        for (int y = 0; y < input.Length - 2; y++)
        {
            for (int x = 0; x < input[y].Length - 2; x++)
            {
                if (input[y + 1][x + 1] != 'A')
                    continue;
                var target = new XMas(
                    input[y + 0][x + 0],
                    input[y + 0][x + 2],
                    input[y + 2][x + 0],
                    input[y + 2][x + 2]);
                if (target == samsam)
                {
#if DEBUG
                    debugOutput[y + 0, x + 0] = 'S';
                    debugOutput[y + 0, x + 2] = 'S';
                    debugOutput[y + 1, x + 1] = 'A';
                    debugOutput[y + 2, x + 0] = 'M';
                    debugOutput[y + 2, x + 2] = 'M';
#endif
                    count++;
                }
                else if (target == masmas)
                {
#if DEBUG
                    debugOutput[y + 0, x + 0] = 'M';
                    debugOutput[y + 0, x + 2] = 'M';
                    debugOutput[y + 1, x + 1] = 'A';
                    debugOutput[y + 2, x + 0] = 'S';
                    debugOutput[y + 2, x + 2] = 'S';
#endif
                    count++;
                }
                else if (target == massam)
                {
#if DEBUG
                    debugOutput[y + 0, x + 0] = 'M';
                    debugOutput[y + 0, x + 2] = 'S';
                    debugOutput[y + 1, x + 1] = 'A';
                    debugOutput[y + 2, x + 0] = 'M';
                    debugOutput[y + 2, x + 2] = 'S';
#endif
                    count++;
                }
                else if (target == sammas)
                {
#if DEBUG
                    debugOutput[y + 0, x + 0] = 'S';
                    debugOutput[y + 0, x + 2] = 'M';
                    debugOutput[y + 1, x + 1] = 'A';
                    debugOutput[y + 2, x + 0] = 'S';
                    debugOutput[y + 2, x + 2] = 'M';
#endif
                    count++;
                }
            }
        }
#if DEBUG
        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input[0].Length; j++)
            {
                Console.Write(debugOutput[i,j]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
#endif
        Console.WriteLine("Count of X-MAS: " + count);
    }

    private readonly struct XMas(char l0X0, char l0X2, char l2X0, char l2X2) : IEquatable<XMas>
    {
        public override string ToString()
        {
            return $"{l0X0}.{l0X2}\n.A.\n{l2X0}.{l2X2}\n";
        }
        public bool Equals(XMas other)
        {
            return Unsafe.As<XMas, long>(ref Unsafe.AsRef(in this)) == Unsafe.As<XMas, long>(ref Unsafe.AsRef(in other));
        }
        public override int GetHashCode()
        {
            return Unsafe.As<XMas, long>(ref Unsafe.AsRef(in this)).GetHashCode();
        }
        public static bool operator ==(XMas left, XMas right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(XMas left, XMas right)
        {
            return !left.Equals(right);
        }
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is XMas other && Equals(other);
        }
    }

    private static void PartOne()
    {
        var rawinput = ReadInput();
       
        var input = rawinput.Split("\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.TrimEnd('\r'))
                            .ToArray();
#if DEBUG
        var debugOutput = new char[input.Length, input[0].Length];
        for (var i = 0; i < input.Length; i++)
        {
            for (var j = 0; j < input[0].Length; j++)
            {
                debugOutput[i, j] = '.';
            }
        }
#endif
        var xmas = "XMAS".ToCharArray();
        var samx = "SAMX".ToCharArray();
        var xmaxTimes = 0;
        
        var xmasVector = Vector64.LoadUnsafe(ref Unsafe.As<char, ushort>(ref xmas[0]));
        var samxvector = Vector64.LoadUnsafe(ref Unsafe.As<char, ushort>(ref samx[0]));
        for (int y = 0; y < input.Length; y++)
        {
            var charArray = input[y].ToCharArray();
            for (int x = 0; x < input[y].Length; x++)
            {
                if (x < input[y].Length - 3)
                {
                    var cmp = Vector64.LoadUnsafe(ref Unsafe.As<char, ushort>(ref charArray[x]));
                    if (cmp == xmasVector)
                    {
#if DEBUG
                        debugOutput[y, x + 0] = (char) cmp.GetElement(0);
                        debugOutput[y, x + 1] = (char) cmp.GetElement(1);
                        debugOutput[y, x + 2] = (char) cmp.GetElement(2);
                        debugOutput[y, x + 3] = (char) cmp.GetElement(3);
#endif
                        xmaxTimes++;
                    }
                    if (cmp == samxvector)
                    {
#if DEBUG
                        debugOutput[y, x + 0] = (char) cmp.GetElement(0);
                        debugOutput[y, x + 1] = (char) cmp.GetElement(1);
                        debugOutput[y, x + 2] = (char) cmp.GetElement(2);
                        debugOutput[y, x + 3] = (char) cmp.GetElement(3);
#endif
                        xmaxTimes++;
                    }
                }
                if (y < input.Length - 3)
                {
                    var cmp2 = Vector64.Create(input[y][x], input[y + 1][x], input[y + 2][x], input[y + 3][x]);
                    if (cmp2 == xmasVector)
                    {
#if DEBUG
                        debugOutput[y + 0, x] = (char) cmp2.GetElement(0);
                        debugOutput[y + 1, x] = (char) cmp2.GetElement(1);
                        debugOutput[y + 2, x] = (char) cmp2.GetElement(2);
                        debugOutput[y + 3, x] = (char) cmp2.GetElement(3);
#endif
                        xmaxTimes++;
                    }
                    if (cmp2 == samxvector)
                    {
#if DEBUG
                        debugOutput[y + 0, x] = (char) cmp2.GetElement(0);
                        debugOutput[y + 1, x] = (char) cmp2.GetElement(1);
                        debugOutput[y + 2, x] = (char) cmp2.GetElement(2);
                        debugOutput[y + 3, x] = (char) cmp2.GetElement(3);
#endif
                        xmaxTimes++;
                    }
                }
                if (x < input[y].Length - 3 && y < input.Length - 3)
                {
                    var cmp3 = Vector64.Create(input[y][x], input[y + 1][x + 1], input[y + 2][x + 2], input[y + 3][x + 3]);
                    if (cmp3 == xmasVector)
                    {
#if DEBUG
                        debugOutput[y + 0, x + 0] = (char) cmp3.GetElement(0);
                        debugOutput[y + 1, x + 1] = (char) cmp3.GetElement(1);
                        debugOutput[y + 2, x + 2] = (char) cmp3.GetElement(2);
                        debugOutput[y + 3, x + 3] = (char) cmp3.GetElement(3);
#endif
                        xmaxTimes++;
                    }
                    if (cmp3 == samxvector)
                    {
#if DEBUG
                        debugOutput[y + 0, x + 0] = (char) cmp3.GetElement(0);
                        debugOutput[y + 1, x + 1] = (char) cmp3.GetElement(1);
                        debugOutput[y + 2, x + 2] = (char) cmp3.GetElement(2);
                        debugOutput[y + 3, x + 3] = (char) cmp3.GetElement(3);
#endif
                        xmaxTimes++;
                    }
                    var cmp4 = Vector64.Create(input[y + 0][x + 3], input[y + 1][x + 2], input[y + 2][x + 1], input[y + 3][x + 0]);
                    if (cmp4 == xmasVector)
                    {
#if DEBUG
                        debugOutput[y + 0, x + 3] = (char) cmp4.GetElement(0);
                        debugOutput[y + 1, x + 2] = (char) cmp4.GetElement(1);
                        debugOutput[y + 2, x + 1] = (char) cmp4.GetElement(2);
                        debugOutput[y + 3, x + 0] = (char) cmp4.GetElement(3);
#endif
                        xmaxTimes++;
                    }
                    if (cmp4 == samxvector)
                    {
#if DEBUG
                        debugOutput[y + 0, x + 3] = (char) cmp4.GetElement(0);
                        debugOutput[y + 1, x + 2] = (char) cmp4.GetElement(1);
                        debugOutput[y + 2, x + 1] = (char) cmp4.GetElement(2);
                        debugOutput[y + 3, x + 0] = (char) cmp4.GetElement(3);
#endif
                        xmaxTimes++;
                    }
                }
            }
        }
#if DEBUG
        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input[0].Length; j++)
            {
                Console.Write(debugOutput[i,j]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
#endif
        Console.WriteLine("Count of XMAS: " + xmaxTimes);
    }
    
    static void Main(string[] args)
    {
        PartOne();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
        Console.Clear();
        PartTwo();
    }
}