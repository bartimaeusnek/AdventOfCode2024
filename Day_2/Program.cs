// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

using Day_2;
string GetInputAsString()
{
    using var txt = File.OpenText("input");
    return txt.ReadToEnd();
}

var debugString =
    """
    48 46 47 49 51 54 56
    1 1 2 3 4 5
    1 2 3 4 5 5
    5 1 2 3 4 5
    1 4 3 2 1
    1 6 7 8 9
    1 2 3 4 3
    9 8 7 6 7
    1 2 3 4
    4 8 3
    5 1 7
    """;

List<int>[] ConvertInputToIntArrays(string input)
{
    var spit = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    var ret = new List<int>[spit.Length];
    for (int i = 0; i < spit.Length; i++)
    {
        var splitLine = spit[i].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        ret[i] = new List<int>(splitLine.Length);
        for (int j = 0; j < splitLine.Length; j++)
        {
            ret[i].Add(int.Parse(splitLine[j]));
        }
    }
    return ret;
}

//Part 1
int SafeReports(List<int>[] input)
{
    int total = 0;
    for (int i = 0; i < input.Length; i++)
    {
        var currentReport = input[i];
        bool? incOrDec = null;
        bool safe = true;
        for (int j = 0; j < currentReport.Count -1; j++)
        {
            if (currentReport[j] > currentReport[j + 1])
            {
                if (!incOrDec.HasValue)
                    incOrDec = true;
                if (!incOrDec.Value)
                {
                    safe = false;
                    break;
                }
                var tmp = currentReport[j] - currentReport[j + 1];
                if (tmp is > 3 or <= 0)
                {
                    safe = false;
                    break;
                }
            }
            else
            {
                if (!incOrDec.HasValue)
                    incOrDec = false;
                if (incOrDec.Value)
                {
                    safe = false;
                    break;
                }
                var tmp = currentReport[j + 1] - currentReport[j];
                if (tmp is > 3 or 0)
                {
                    safe = false;
                    break;
                }
            }
        }
        if (safe)
            total++;
    }
    return total;
}

//Part 1
Console.WriteLine(SafeReports(ConvertInputToIntArrays(GetInputAsString())));
//Part 2
var inputLists = ConvertInputToIntArrays(GetInputAsString());
var board = new List<Report>();
for (int index = 0; index < inputLists.Length; index++)
{
    List<int> x = inputLists[index];
    var report = new Report(x);
#if DEBUG
    Console.WriteLine(index);
    Console.WriteLine(report);
#endif
    report = report.ApplyDampeners();
    board.Add(report);
#if DEBUG
    Console.WriteLine(report);
    Console.ReadKey();
    Console.Clear();
#endif
}
Console.WriteLine(board.Count(x => x.IsValid));


