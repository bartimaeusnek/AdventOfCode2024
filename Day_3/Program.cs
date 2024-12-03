// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_3;

using System.Text.RegularExpressions;

public partial class Program
{
    private static string ReadInput()
    {
        using var reader = new StreamReader("input");
        return reader.ReadToEnd();
    }
    
    [GeneratedRegex(@"mul\((\d+),(\d+)\)")]
    private static partial Regex MultiplyRegex();
    private static void PartOne()
    {
        var str = ReadInput();
        var regex = MultiplyRegex();
        var muls = regex.Matches(str);
        Console.WriteLine(muls.Select(m => int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value)).Sum());
    }
    
    [GeneratedRegex(@"(?<enabled>do\(\))|(?<disabled>don't\(\))|(?<mul>\bmul\((\d+),(\d+)\))")]
    private static partial Regex NoDontRegex();
    private static void PartTwoOne()
    {
        var str = ReadInput();
        var regex = NoDontRegex();
        var matches = regex.Matches(str);
        var sum = 0;
        var enabled = true;
        foreach (var match in matches.Select(x => x))
        {
            if (match.Groups["enabled"].Success)
            {
                enabled = true;
            }
            else if (match.Groups["disabled"].Success)
            {
                enabled = false;
            }
            else if (match.Groups["mul"].Success && enabled)
            {
                sum += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
            }
        }
        Console.WriteLine(sum);
    }
    
    public static void Main()
    {
        PartOne();
        PartTwoOne();
    }
}