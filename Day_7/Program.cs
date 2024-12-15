// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_7;

using System.Diagnostics;

class Program
{
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    private static string DebugInput =
        """
        190: 10 19
        3267: 81 40 27
        83: 17 5
        156: 15 6
        7290: 6 8 6 15
        161011: 16 10 13
        192: 17 8 14
        21037: 9 7 18 13
        292: 11 6 16 20
        """;

    private static IEnumerable<Equasion> GetEquasionsFromInput(string input)
    {
        foreach (string se in input.Trim('\r')
                                   .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var line = se.Trim('\r');
            var splitLine = line.Split(':');
            var result = ulong.Parse(splitLine[0]);
            var numbers = splitLine[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            yield return new Equasion(result, GetNumbersFromSplitInput(numbers));
        }
    }

    private static IEnumerable<ulong> GetNumbersFromSplitInput(string[] input)
    {
        foreach (string number in input)
            yield return ulong.Parse(number);
    }

    private static IEnumerable<Task<ulong>> StartCalculations(Equasion[] equasions, bool part2)
    {
        foreach (var equasion in equasions)
            yield return Task.Run(() => equasion.SolveAll(part2) ? equasion.Result : 0ul);
    }

    static async Task Main(string[] args)
    {
        var sw = Stopwatch.StartNew();
        var input = GetEquasionsFromInput(
#if DEBUG
            DebugInput
#else
            await ReadInputAsync()
#endif
        ).ToArray();

        ulong value = 0;
        foreach (ulong @ulong in await Task.WhenAll(StartCalculations(input, false)))
        {
            value += @ulong;
        }
        Console.WriteLine(value);
        value = 0;
        foreach (ulong @ulong in await Task.WhenAll(StartCalculations(input, true)))
        {
            value += @ulong;
        }
        Console.WriteLine(value);
        sw.Stop();
        Console.WriteLine(sw.Elapsed);
    }
}