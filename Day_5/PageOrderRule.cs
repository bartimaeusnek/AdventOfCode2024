// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_5;

public readonly struct PageOrderRule
{
    private readonly int _a;
    private readonly int _b;
    public PageOrderRule(int a, int b)
    {
        _a = a;
        _b = b;
        AllRules.Enqueue(this);
    }
    private bool Matches(int[] arrays)
    {
        return !arrays.Contains(_a) || !arrays.Contains(_b) || Array.IndexOf(arrays, _a) < Array.IndexOf(arrays, _b);
    }
    
    private static readonly Queue<PageOrderRule> AllRules = new Queue<PageOrderRule>();

    private void Swap(int[] arrays)
    {
        if (!arrays.Contains(_a) || !arrays.Contains(_b))
            return;
        var idxA = Array.IndexOf(arrays, _a);
        var idxB = Array.IndexOf(arrays, _b);
        if (idxA > idxB)
        {
            (arrays[idxA], arrays[idxB]) = (arrays[idxB], arrays[idxA]);
        }
    }
    
    public static void SwapAll(int[] arrays)
    {
        foreach (var allRule in AllRules)
        {
            allRule.Swap(arrays);
        }
    }
    
    public static bool MatchesAll(int[] arrays)
    {
        return AllRules.All(x => x.Matches(arrays));
    }

    public static int MiddleNumber(int[] arrays)
    {
        return arrays[arrays.Length / 2];
    }
}