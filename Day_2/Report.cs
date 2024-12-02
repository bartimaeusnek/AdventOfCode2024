// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

//Part 2
namespace Day_2;

using System.Diagnostics;
using System.Text;

public enum Direction
{
    Ascending,
    Descending,
    None
}

public class Report
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("###########################");
        sb.AppendLine();
        sb.AppendLine(LevelDirection.ToString());
        for (int index = 0; index < Levels.Count; index++)
        {
            var level = Levels[index];
            sb.Append(level.Value);
            sb.Append(' ');
            if (index < Levels.Count - 1)
            {
                sb.Append('(');
                sb.Append(level.DistanceForward is < 1 or > 3 ? level.DistanceForward + " !ERROR!" : level.DistanceForward);
                sb.Append(',');
                sb.Append(level.ForwardDirection != LevelDirection ? level.ForwardDirection + " !ERROR!" : level.ForwardDirection);
                sb.Append(')');
                
                sb.Append(' ');
            }
        }
        sb.Append(IsValid);
        sb.AppendLine();
        sb.Append("###########################");
        return sb.ToString();
    }

    private readonly List<Level> Levels = new List<Level>();
    
    private Report(List<Level> levels)
    {
        Levels = levels.Select(x => new Level(x.Value, x.Index, this)).ToList();
    }

    public Report(List<int> levels)
    {
        for (var i = 0; i < levels.Count; i++)
        {
            Levels.Add(new Level(levels[i], i, this));
        }
    }

    public Level? GetLevel(int levelNumber)
    {
        if (levelNumber < 0 || levelNumber >= Levels.Count)
            return null;
        return Levels[levelNumber];
    }
    
    public int Length => Levels.Count;
    public Direction LevelDirection
    {
        get
        {
            var directions = Levels
                             .Select(x => x.ForwardDirection)
                             .Where(x => x != Direction.None)
                             .Where(x => x != null)
                             .ToList();
            var noAsc = directions.Count(x => x == Direction.Ascending);
            var noDesc = directions.Count(x => x == Direction.Descending);
            return noAsc < noDesc ? Direction.Descending : noDesc < noAsc ? Direction.Ascending : Direction.None;
        }
    }

    public Report ApplyDampeners()
    {
        //fuck this, im brute-foceing.
        if (IsValid)
            return this;
        
        var cnt = Levels.Count;
        for (int i = cnt - 1; i >= 0; i--)
        {
            var report = new Report(Levels);
            report.RemoveIndex(i);
            if (report.IsValid)
                return report;
        }
        return this;

        // if (LevelDirection is Direction.None)
        // {
        //     for (var i = Levels.Count - 1; i >= 0; i--)
        //     {
        //         var current = Levels[i];
        //         if (current.DistanceBackward is not null)
        //         {
        //             if (Math.Abs(current.DistanceBackward.Value) is >= 1 and <= 3)
        //                 continue;
        //             if (current.Previous?.Previous is not null)
        //             {
        //                 if (Math.Abs(current.Value - current.Previous.Previous.Value) is >= 1 and <= 3)
        //                 {
        //                     RemoveIndex(current.Index - 1);
        //                     continue;
        //                 }
        //             }
        //             RemoveIndex(current.Index);
        //             return this;
        //         }
        //     }
        // }
        //
        // var potentiallyInvalid = new List<Level>();
        // for (var i = Levels.Count - 1; i >= 0; i--)
        // {
        //     var current = Levels[i];
        //     if (!current.IsValid)
        //     {
        //         if (current.IsForwardValid is null && current.IsBackwardValid is false 
        //          || current.IsForwardValid is false && current.IsBackwardValid is null
        //          || current.IsForwardValid is false && current.IsBackwardValid is false)
        //         {
        //             RemoveIndex(current.Index);
        //             return this;
        //         }
        //         potentiallyInvalid.Add(current);
        //     }
        // }
        // foreach (var current in potentiallyInvalid)
        // {
        //     if (current.IsForwardValid is true)
        //     {
        //         if (current.Previous!.Previous is null)
        //         {
        //             RemoveIndex(current.Index);
        //             return this;
        //         }
        //         if (current.Previous!.IsBackwardValid is true)
        //         {
        //             RemoveIndex(current.Index);
        //             return this;
        //         }
        //     }
        //     if (current.IsBackwardValid is true)
        //     {
        //         if (current.Next!.Next is null)
        //         {
        //             RemoveIndex(current.Index);
        //             return this;
        //         }
        //         if (current.Next!.IsForwardValid is true)
        //         {
        //             RemoveIndex(current.Index);
        //             return this;
        //         }
        //     }
        // }
        //
        // if (potentiallyInvalid.Count == 1)
        // {
        //     RemoveIndex(potentiallyInvalid.First().Index);
        //     return this;
        // }
        //
        // return this;
    }
    
    public bool IsValid => Levels.TrueForAll(x => x.IsValid);

    private void RemoveIndex(int index)
    {
#if DEBUG
        var val = Levels[index].Value;        
#endif
        Levels.RemoveAt(index);
        UpdateIndexes();
#if DEBUG
        if (!IsValid)
            return;
        Debug.WriteLine("Removed Index: {0} Value: {1}", index, val);
        foreach (var level in Levels)
        {
            Debug.Write(' ');
            Debug.Write(level.Value);
        }
        Debug.Write('\n');
#endif
    }
    
    private void UpdateIndexes()
    {
        for (var i = 0; i < Levels.Count; i++)
        {
            Levels[i].Index = i;
        }
    }
    
}

public class Level
{
    public int Value;
    public int Index;
    public Report Parent;
    
    public Level? Next => Parent.GetLevel(Index + 1);
    public Level? Previous => Parent.GetLevel(Index - 1);
    
    public Direction? ForwardDirection
    {
        get
        {
            if (Next == null)
                return null;
            if (this.Value < Next.Value)
                return Direction.Ascending;
            else if (this.Value > Next.Value)
                return Direction.Descending;
            else
                return Direction.None;
        }
    }
    public Direction? BackwardDirection
    {
        get
        {
            if (Previous == null)
                return null;
            if (Previous.Value < this.Value)
                return Direction.Ascending;
            else if (Previous.Value > this.Value)
                return Direction.Descending;
            else
                return Direction.None;
        }
    }

    public int? DistanceForward
    {
        get
        {
            if (Next == null)
                return null;
            return Parent.LevelDirection switch
            {
                Direction.Ascending => Next.Value - Value,
                Direction.Descending => Value - Next.Value,
                Direction.None => Next.Value - Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public int? DistanceBackward
    {
        get
        {
            if (Previous == null)
                return null;
            return Parent.LevelDirection switch
            {
                Direction.Ascending => Value - Previous.Value,
                Direction.Descending => Previous.Value - Value,
                Direction.None => Previous.Value - Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public Level(int value, int index, Report parent)
    {
        Value = value;
        Index = index;
        Parent = parent;
    }

    public bool? IsForwardValid
    {
        get
        {
            if (Next is null)
                return null;
            
            var directionCheck = ForwardDirection == Parent.LevelDirection;
            var distanceCheck = DistanceForward <= 3 && DistanceForward > 0;
            return directionCheck && distanceCheck;
        }
    }
    
    public bool? IsBackwardValid
    {
        get
        {
            if (Previous is null)
                return null;
            
            var directionCheck = BackwardDirection == Parent.LevelDirection;
            var distanceCheck = DistanceBackward <= 3 && DistanceBackward > 0;
            return directionCheck && distanceCheck;
        }
    }
    
    public bool IsValid => IsForwardValid is null or true 
                        && IsBackwardValid is null or true;
}