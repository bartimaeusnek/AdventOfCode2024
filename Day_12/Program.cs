namespace Day_12;

using System.Diagnostics;

class Program
{
    private static string SimplestExample = """
                                            AAAA
                                            BBCD
                                            BBCC
                                            EEEC
                                            """;
    
    private static string SimpleExample = """
                                          OOOOO
                                          OXOXO
                                          OOOOO
                                          OXOXO
                                          OOOOO
                                          """;

    private static string Example = """
                                    RRRRIICCFF
                                    RRRRIICCCF
                                    VVRRRCCFFF
                                    VVRCCCJFFF
                                    VVVVCJJCFE
                                    VVIVCCJJEE
                                    VVIIICJJEE
                                    MIIIIIJJEE
                                    MIIISIJEEE
                                    MMMISSJEEE
                                    """;

    //Thanks to https://www.reddit.com/r/adventofcode/comments/1hcib0z/2024_day_12_yet_another_test_case/
    private static string MerryChristmasExample = """
                                            XOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOX
                                            OXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXO
                                            XXXXXXXXXXXXXXXMXXXMXEEEEEXRRRRXXRRRRXXYXXXYXXXXXXXXXXXXXXX
                                            OXXXXXXXXXXXXXXMMXMMXEXXXXXRXXXRXRXXXRXYXXXYXXXXXXXXXXXXXXO
                                            XXXXXXXXXXXXXXXMXMXMXEEEEEXRRRRXXRRRRXXXYXYXXXXXXXXXXXXXXXX
                                            OXXXXXXXXXXXXXXMXXXMXEXXXXXRXXXRXRXXXRXXXYXXXXXXXXXXXXXXXXO
                                            XXXXXXXXXXXXXXXMXXXMXEEEEEXRXXXRXRXXXRXXXYXXXXXXXXXXXXXXXXX
                                            OXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXO
                                            XXXXCCCXXHXXXHXRRRRXXIIIIIXXSSSSXTTTTTXMXXXMXXAAAXXXSSSSXXX
                                            OXXCXXXCXHXXXHXRXXXRXXXIXXXSXXXXXXXTXXXMMXMMXAXXXAXSXXXXXXO
                                            XXXCXXXXXHHHHHXRRRRXXXXIXXXXSSSXXXXTXXXMXMXMXAAAAAXXSSSXXXX
                                            OXXCXXXCXHXXXHXRXXXRXXXIXXXXXXXSXXXTXXXMXXXMXAXXXAXXXXXSXXO
                                            XXXXCCCXXHXXXHXRXXXRXIIIIIXSSSSXXXXTXXXMXXXMXAXXXAXSSSSXXXX
                                            OXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXO
                                            XOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOXOX
                                           """;
    
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    public static void ParseInput(string input)
    {
        var split = input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        Cell.AllCells = new Cell[split.Length][];
        for (int y = 0; y < split.Length; y++)
        {
            Cell.AllCells[y] = new Cell[split[y].Length];
            for (int x = 0; x < split[y].Length; x++)
            {
                Cell.AllCells[y][x] = new Cell
                {
                    X = x,
                    Y = y,
                    Value = split[y][x]
                };
            }
        }
    }

    private static int GetSumPart1()
    {
        return Cell.AllCells
                   .SelectMany(x => x)
                   .Select(x => x.Area)
                   .Distinct()
                   .Sum(x => x.Price);
    }
    private static int GetSumPart2()
    {
        return Cell.AllCells
                   .SelectMany(x => x)
                   .Select(x => x.Area)
                   .Distinct()
                   .Sum(x => x.PricePart2);
    }
    
#if DEBUG
    private static int DebugGetSumPart2()
    {
        int sum = 0;
        HashSet<Area2d> set = new HashSet<Area2d>();
        foreach (Cell[] x in Cell.AllCells)
            foreach (Cell cell in x)
            {
                var x1 = cell.Area;
                if (set.Add(x1))
                {
                    Console.WriteLine($"A region of {x1.Area[0].Value} plants with price {x1.Area.Count} * {x1.Sides} = {x1.PricePart2}.");
                    
                    sum += x1.PricePart2;
                }
            }
        return sum;
    }

    public static void DebugRegions()
    {
        foreach (var area2d in Cell.AllCells
                              .SelectMany(x => x)
                              .Select(x => x.Area)
                              .Distinct())
        {
            Console.Clear();
            Console.WriteLine(area2d.ToString());
            area2d.Print();
            Console.ReadKey();
        }
    }
    
    public static void DebugAssert(string input, int expected, int expected2)
    {
        Console.Clear();
        ParseInput(input);
        Area2d.AllAreas.Clear();
        var sumPart1 = GetSumPart1();
        var sumPart2 = DebugGetSumPart2();
        Debug.Assert(expected == sumPart1, $"Part1: Expected: {expected} == sum, Actual: {sumPart1} == sum");
        Debug.Assert(expected2 == sumPart2, $"Part2: Expected: {expected2} == sum, Actual: {sumPart2} == sum");
    }
#endif
    
    static async Task Main(string[] args)
    {
#if DEBUG
        DebugAssert(SimplestExample, 140, 80);
        DebugAssert(SimpleExample, 772, 436);
        DebugAssert(Example, 1930, 1206);
        DebugAssert(MerryChristmasExample, 426452, 307122);
#endif
        ParseInput(await ReadInputAsync());
        Area2d.AllAreas.Clear();
        var sum = GetSumPart1();
        Console.WriteLine("Part 1: " + sum);
        sum = GetSumPart2();
        Console.WriteLine("Part 2: " + sum);
    }

    public class Area2d : IEquatable<Area2d>
    {
        public override string ToString()
        {
            return $"{Area.First().Value}: {Area.Count} * {Walls} = {Price}";
        }

        public void Print()
        {
            for (var y = 0; y < Cell.AllCells.Length; y++)
            {
                for (var x = 0; x < Cell.AllCells[y].Length; x++)
                {
                    if (Area.Contains(Cell.AllCells[y][x]))
                        Console.Write(Cell.AllCells[y][x]);
                    else
                        Console.Write(' ');
                }
                Console.WriteLine();
            }
        }
        
        public static Dictionary<Cell, Area2d> AllAreas = new();
        public bool Equals(Area2d? other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Area.SequenceEqual(other.Area);
        }
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Area2d)obj);
        }
        public override int GetHashCode()
        {
            return Area.Select(x => x.GetHashCode())
                       .Aggregate(0, HashCode.Combine);
        }
        public static bool operator ==(Area2d? left, Area2d? right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Area2d? left, Area2d? right)
        {
            return !Equals(left, right);
        }
        public List<Cell> Area;
        public int Walls => Area.Select(x => x.Walls).Sum();

        public int Sides
        {
            get
            {
                var y = 0;
                var totalWalls = 0;
                WallDirection wallsToSkip = 0;
                
                //left-to-right check for top/bottom walls
                for (y = 0; y < Cell.AllCells.Length; y++)
                {
                    wallsToSkip = 0;
                    for (var x = 0; x < Cell.AllCells[y].Length; x++)
                    {
                        if (!Area.Contains(Cell.AllCells[y][x]))
                        {
                            wallsToSkip = 0;
                            continue;
                        }

                        var currentWall = Cell.AllCells[y][x].WallsPt2;
                        if (currentWall.HasFlag(WallDirection.TOP))
                        {
                            if (!wallsToSkip.HasFlag(WallDirection.TOP))
                            {
                                totalWalls++;
                                wallsToSkip |= WallDirection.TOP;
                            }
                        }
                        else
                        {
                            wallsToSkip &= ~WallDirection.TOP;
                        }

                        if (currentWall.HasFlag(WallDirection.BOTTOM))
                        {
                            if (!wallsToSkip.HasFlag(WallDirection.BOTTOM))
                            {
                                totalWalls++;
                                wallsToSkip |= WallDirection.BOTTOM;
                            }
                        }
                        else
                        {
                            wallsToSkip &= ~WallDirection.BOTTOM;
                        }
                    }
                }

                //top-to-bottom check for left/right walls
                y = 0;
                for (var x = 0; x < Cell.AllCells[y].Length; x++)
                {
                    wallsToSkip = 0;
                    for (; y < Cell.AllCells.Length; y++)
                    {
                        if (!Area.Contains(Cell.AllCells[y][x]))
                        {
                            wallsToSkip = 0;
                            continue;
                        }

                        var currentWall = Cell.AllCells[y][x].WallsPt2;
                        if (currentWall.HasFlag(WallDirection.LEFT))
                        {
                            if (!wallsToSkip.HasFlag(WallDirection.LEFT))
                            {
                                totalWalls++;
                                wallsToSkip |= WallDirection.LEFT;
                            }
                        }
                        else
                        {
                            wallsToSkip &= ~WallDirection.LEFT;
                        }

                        if (currentWall.HasFlag(WallDirection.RIGHT))
                        {
                            if (!wallsToSkip.HasFlag(WallDirection.RIGHT))
                            {
                                totalWalls++;
                                wallsToSkip |= WallDirection.RIGHT;
                            }
                        }
                        else
                        {
                            wallsToSkip &= ~WallDirection.RIGHT;
                        }
                    }
                    y = 0;
                }
                return totalWalls;
            }
        }

        public int Price => Walls * Area.Count;
        
        public int PricePart2 => Sides * Area.Count;
    }

    [Flags]
    public enum WallDirection
    {
        LEFT = 1,
        TOP = 2,
        RIGHT = 4,
        BOTTOM = 8,
    }
    
    public class Cell : IEquatable<Cell>
    {
        public static Cell[][] AllCells;
        public char Value;
        public int X;
        public int Y;
        
        public override string ToString()
        {
            return Value.ToString();
        }

        public bool Equals(Cell? other)
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
            return Equals((Cell)obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Value, X, Y);
        }
        public static bool operator ==(Cell? left, Cell? right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Cell? left, Cell? right)
        {
            return !Equals(left, right);
        }
        public int Walls
        {
            get
            {
                int walls = 0;
                
                if (X - 1 < 0 || AllCells[Y][X - 1].Value != Value)
                    ++walls;
                if (Y - 1 < 0 || AllCells[Y - 1][X].Value != Value)
                    ++walls;
                if (X + 1 >= AllCells[Y].Length || AllCells[Y][X + 1].Value != Value)
                    ++walls;
                if (Y + 1 >= AllCells.Length || AllCells[Y + 1][X].Value != Value)
                    ++walls;
                
                return walls;
            }
        }

        public WallDirection WallsPt2
        {
            get
            {
                int walls = 0;
                
                if (X - 1 < 0 || AllCells[Y][X - 1].Value != Value)
                    walls |= 0b0001;
                if (Y - 1 < 0 || AllCells[Y - 1][X].Value != Value)
                    walls |= 0b0010;
                if (X + 1 >= AllCells[Y].Length || AllCells[Y][X + 1].Value != Value)
                    walls |= 0b0100;
                if (Y + 1 >= AllCells.Length || AllCells[Y + 1][X].Value != Value)
                    walls |= 0b1000;
                
                return (WallDirection) walls;
            }
        }
        
        public Area2d Area
        {
            get
            {
                if (Area2d.AllAreas.TryGetValue(this, out var area))
                    return area;
                
                area = new Area2d
                {
                    Area = GetAreaCells()
                };
                area.Area.ForEach(x => Area2d.AllAreas[x] = area);
                return area;
            }
        }

        private void GetAreaCells(List<Cell> cells)
        {
            cells.Add(this);
            if (X - 1 >= 0 && AllCells[Y][X - 1].Value == Value && cells.Contains(AllCells[Y][X - 1]) is false)
            {
                AllCells[Y][X - 1].GetAreaCells(cells);
            }
            if (Y - 1 >= 0 && AllCells[Y - 1][X].Value == Value && cells.Contains(AllCells[Y - 1][X]) is false)
            {
                AllCells[Y - 1][X].GetAreaCells(cells);
            }
            if (X + 1 < AllCells[Y].Length && AllCells[Y][X + 1].Value == Value && cells.Contains(AllCells[Y][X + 1]) is false)
            {
                AllCells[Y][X + 1].GetAreaCells(cells);
            }
            if (Y + 1 < AllCells.Length && AllCells[Y + 1][X].Value == Value && cells.Contains(AllCells[Y + 1][X]) is false)
            {
                AllCells[Y + 1][X].GetAreaCells(cells);
            }
        }
        
        public List<Cell> GetAreaCells()
        {
            var cells = new List<Cell>();
            GetAreaCells(cells);
            return cells;
        }
    }
}