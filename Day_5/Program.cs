// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_5;

class Program
{
    private const string DebugInput = """
                                      47|53
                                      97|13
                                      97|61
                                      97|47
                                      75|29
                                      61|13
                                      75|53
                                      29|13
                                      97|29
                                      53|29
                                      61|53
                                      97|53
                                      61|29
                                      47|13
                                      75|47
                                      97|75
                                      47|61
                                      75|61
                                      47|29
                                      75|13
                                      53|13

                                      75,47,61,53,29
                                      97,61,53,29,13
                                      75,29,13
                                      75,97,47,61,53
                                      61,13,29
                                      97,13,75,29,47
                                      """;

    private static string ReadInput()
    {
        using var reader = new StreamReader("input");
        return reader.ReadToEnd();
    }
    public static void ParsePoRules(string input)
    {
        input.Trim('\r')
             .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
             .Where(x => x.Contains('|'))
             .Select(x => x.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
             .ToList()
             .ForEach(x => _ = new PageOrderRule(int.Parse(x[0]), int.Parse(x[1])));
    }
    
    public static int[][] ParseInput(string input)
    {
        return input.Trim('\r')
             .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
             .Where(x => !x.Contains('|'))
             .Select(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
             .Select(Inputs)
             .Select(x => x.ToArray())
             .ToArray();
    }

    static IEnumerable<int> Inputs(string[] input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            yield return int.Parse(input[i]);
        }
    }
    
    static void Main(string[] args)
    {
        var input = ReadInput();
        ParsePoRules(input);
        var di = ParseInput(input);
        var countPart1 = 0;
        var countPart2 = 0;
        foreach (int[] intse in di)
        {
            if (PageOrderRule.MatchesAll(intse))
            {
                countPart1 += PageOrderRule.MiddleNumber(intse);
            }
            else
            {
                do
                {
                    PageOrderRule.SwapAll(intse);
                } while (!PageOrderRule.MatchesAll(intse));
                countPart2 += PageOrderRule.MiddleNumber(intse);
            }
        }
        Console.WriteLine("Part 1: " + countPart1);
        Console.WriteLine("Part 2: " + countPart2);
    }
}