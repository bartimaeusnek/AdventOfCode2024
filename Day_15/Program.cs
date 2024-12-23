// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_15;

using System.Diagnostics;

internal static class Program
{
    public static Tile[][] Field;
    public static Box[][] Boxes;
    public static List<Robot> Robots = new List<Robot>(); 
    public record struct Tile(bool HasWall);

    public record Robot(int X, int Y) : Moveable(X, Y);

    public abstract record Moveable(int X, int Y)
    {
        public int X { get; set; } = X;
        public int Y { get; set; } = Y;

        public bool Move(char direction) => direction switch
        {
            '<' => MoveDirection(direction, -1, 0),
            '>' => MoveDirection(direction, 1, 0),
            'v' => MoveDirection(direction, 0, 1), 
            '^' => MoveDirection(direction, 0, -1),
            _ => false
        };

        protected internal virtual bool MoveDirection(char direction, int directionX, int directionY) 
            => !Field[Y + directionY][X + directionX].HasWall && (Boxes[Y + directionY][X + directionX] == null 
               ? ActuallyMove(directionX, directionY) 
               : Boxes[Y + directionY][X + directionX].Move(direction) && ActuallyMove(directionX, directionY));

        protected internal bool ActuallyMove(int directionX, int directionY)
        {
            switch (this)
            {
                case Robot:
                    X += directionX;
                    Y += directionY;
                    return true;
                case BoxPart2:
                case Box:
                    Boxes[Y][X] = null;
                    Boxes[Y + directionY][X + directionX] = (Box)this;
                    X += directionX;
                    Y += directionY;
                    return true;
                default:
                    return false;
            }
        }
    }

    public record BoxPart2(int X, int Y, bool MainPart) : Box(X, Y)
    {
        protected internal override bool MoveDirection(char direction, int directionX, int directionY)
        {
            if (directionX != 0)
            {
                return base.MoveDirection(direction, directionX, directionY);
            }

            var x = MainPart ? X + 1 : X - 1;
            var wallcheck = !Field[Y + directionY][X].HasWall && !Field[Y + directionY][x].HasWall;
            if (!wallcheck)
                return false;
            if (Boxes[Y + directionY][X] == null && Boxes[Y + directionY][x] == null)
            {
                return Boxes[Y][x].ActuallyMove(0, directionY) && ActuallyMove(0, directionY);
            }
            if ((Boxes[Y + directionY][X] == null || Boxes[Y + directionY][X] != null && Boxes[Y + directionY][X].MoveDirection(direction, directionX, directionY)) 
             && (Boxes[Y + directionY][x] == null || Boxes[Y + directionY][x] != null && Boxes[Y + directionY][x].MoveDirection(direction, directionX, directionY)))
            {
                return Boxes[Y][x].ActuallyMove(0, directionY) && ActuallyMove(0, directionY);
            }
            return false;
        }

        public override int GetGps()
        {
            return MainPart ? 100 * Y + X : 0;
        }
        
        public override char GetChar()
        {
            return MainPart ? '[' : ']';
        }
    }
    
    public record Box(int X, int Y) : Moveable(X, Y)
    {
        public virtual int GetGps() => 100 * Y + X;

        public virtual char GetChar() => 'O';
    }

    public static Queue<char> Instructions = new Queue<char>();
    
    public static string SmallerExample = """
                                      ########
                                      #..O.O.#
                                      ##@.O..#
                                      #...O..#
                                      #.#.O..#
                                      #...O..#
                                      #......#
                                      ########

                                      <^^>>>vv<v>>v<<
                                      """;

    public static string Example = """
                                   ##########
                                   #..O..O.O#
                                   #......O.#
                                   #.OO..O.O#
                                   #..O@..O.#
                                   #O#..O...#
                                   #O..O..O.#
                                   #.OO.O.OO#
                                   #....O...#
                                   ##########

                                   <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
                                   vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
                                   ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
                                   <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
                                   ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
                                   ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
                                   >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
                                   <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
                                   ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
                                   v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
                                   """;
    
    public const string ExtraTestCase = """
                                        #######
                                        #.....#
                                        #..O..#
                                        #.OO..#
                                        #.O.O.#
                                        #@OO..#
                                        #..O..#
                                        #.....#
                                        #.....#
                                        #######
                                        
                                        >><^^>^^>>v
                                        """;
    
    public const string ExtraTestCase2 = """
                                        #######
                                        #.....#
                                        #..O..#
                                        #.OO..#
                                        #@....#
                                        #.....#
                                        #.....#
                                        #.....#
                                        #.....#
                                        #######

                                        ^>>v>^^
                                        """;
    public const string ExtraTestCase3 = """
                                         #######
                                         #.....#
                                         #..O..#
                                         #.OO..#
                                         #..@..#
                                         #.....#
                                         #.....#
                                         #.....#
                                         #.....#
                                         #######

                                         ^^^^
                                         """;
    
    public const string ExtraTestCase4 = """
                                         #######
                                         #.....#
                                         #.O...#
                                         #.OO..#
                                         #..@..#
                                         #.....#
                                         #.....#
                                         #.....#
                                         #.....#
                                         #######

                                         ^^^^
                                         """;
    public const string ExtraTestCase5 = """
                                         #######
                                         #.....#
                                         #@OO..#
                                         #..O..#
                                         #.....#
                                         #.....#
                                         #.....#
                                         #.....#
                                         #.....#
                                         #######

                                         >>vv>>^^
                                         """;
    

    public static async Task<string> GetInput()
    {
        using var input = File.OpenText("input");
        return await input.ReadToEndAsync();
    }
    
    public static void ReadInput(string input)
    {
        var rawSanitizedInput = input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
             .Select(x => x.Trim('\r'))
             .ToArray();
        var field = rawSanitizedInput.Where(x => x[0] == '#').ToArray();
        Field = new Tile[field.Length][];
        Boxes = new Box[field.Length][];
        for (int y = 0; y < field.Length; y++)
        {
            Field[y] = new Tile[field[y].Length];
            Boxes[y] = new Box[field[y].Length];
            for (int x = 0; x < field[y].Length; x++)
            {
                Field[y][x] = new Tile(field[y][x] == '#');
                if (field[y][x] == 'O')
                {
                    Boxes[y][x] = new Box(x, y);
                }
                if (field[y][x] == '@')
                {
                    Robots.Add(new Robot(x, y));
                }
            }
        }
        foreach (char c in rawSanitizedInput.Where(x => x[0] != '#').SelectMany(x => x))
        {
            Instructions.Enqueue(c);
        }
    }
    
    public static void ReadInputPart2(string input)
    {
        var rawSanitizedInput = input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                     .Select(x => x.Trim('\r'))
                                     .ToArray();
        
        var field = rawSanitizedInput.Where(x => x[0] == '#').ToArray();
        Field = new Tile[field.Length][];
        Boxes = new BoxPart2[field.Length][];
        for (int y = 0; y < field.Length; y++)
        {
            Field[y] = new Tile[field[y].Length * 2];
            Boxes[y] = new BoxPart2[field[y].Length * 2];
            
            for (int x = 0; x < field[y].Length * 2; x += 2)
            {
                Field[y][x] = new Tile(field[y][x/2] == '#');
                Field[y][x + 1] = new Tile(field[y][x/2] == '#');
                
                if (field[y][x/2] == 'O')
                {
                    Boxes[y][x] = new BoxPart2(x, y, true);
                    Boxes[y][x + 1] = new BoxPart2(x + 1, y, false);
                }
                if (field[y][x/2] == '@')
                {
                    Robots.Add(new Robot(x, y));
                }
            }
        }
        foreach (char c in rawSanitizedInput.Where(x => x[0] != '#').SelectMany(x => x))
        {
            Instructions.Enqueue(c);
        }
    }

    public static void PrintField()
    {
        for (var y = 0; y < Field.Length; y++)
        {
            for (var x = 0; x < Field[y].Length; x++)
            {
                if (Boxes[y][x] != null)
                {
                    Console.Write('O');
                    continue;
                }
                if (Field[y][x].HasWall)
                {
                    Console.Write('#');
                    continue;
                }
                if (Robots.Any(r => r.X == x && r.Y == y))
                {
                    Console.Write('@');
                    continue;
                }
                Console.Write('.');
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    
    public static void PrintFieldPart2()
    {
        for (var y = 0; y < Field.Length; y++)
        {
            for (var x = 0; x < Field[y].Length; x++)
            {
                if (Boxes[y][x] != null)
                {
                    var p2 = (BoxPart2)Boxes[y][x];
                    Console.Write(p2.GetChar());
                    continue;
                }
                if (Field[y][x].HasWall)
                {
                    Console.Write('#');
                    continue;
                }
                if (Robots.Any(r => r.X == x && r.Y == y))
                {
                    Console.Write('@');
                    continue;
                }
                Console.Write('.');
            }
            if (y != Field.Length - 1)
                Console.WriteLine();
        }
    }
    
    static async Task Main(string[] args)
    {
        // ReadInput(await GetInput());
        // PrintField();
        // var robot = Robots.First();
        // int i = 0;
        // foreach (char instruction in Instructions)
        // {
        //     if (i++ == "<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^".Length)
        //     {
        //         Console.WriteLine();
        //         i = 1;
        //     }
        //     Console.Write(instruction);
        // }
        //
        // foreach (char instruction in Instructions)
        // {
        //     // Console.ReadKey();
        //     // Console.Clear();
        //     // Console.WriteLine("Current Instruction: " + instruction);
        //     robot.Move(instruction);
        //     // PrintField();
        // }
        //
        // Console.WriteLine(Boxes.SelectMany(x => x).Where(x => x != null)
        //                         .Select(x => x.GetGps())
        //                         .Sum());
        //

        String[] extraTestCases = [await GetInput()];
        
        foreach (string extraTestCase in extraTestCases)
        {
            Field = null;
            Boxes = null;
            Instructions.Clear();
            Robots.Clear();
            ReadInputPart2(extraTestCase);
            PrintFieldPart2();
            var robot = Robots.First();
            foreach (char instruction in Instructions)
            {
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("Current Instruction: " + instruction);
                var preMoveBoxes = Boxes.SelectMany(x => x).Where(x => x is not null).ToArray();
                var boxCoords = preMoveBoxes.Select(x => (x.X, x.Y)).ToArray();
                robot.Move(instruction);
                PrintFieldPart2();
            
                //no box within another box
                Debug.Assert(preMoveBoxes.Count(x => x is not null) == Boxes.SelectMany(x => x).Count(x => x is not null));
                Debug.Assert(preMoveBoxes.Count(x => x is not null) == Boxes.SelectMany(x => x).Where(x => x is not null).Select(x => (x.X, x.Y)).Distinct().Count());
            
                //No Box or robot in wall
                for (int y = 0; y < Boxes.Length; y++)
                {
                    for (int x = 0; x < Boxes[y].Length; x++)
                    {
                        if (Boxes[y][x] != null)
                            Debug.Assert(!Field[y][x].HasWall);
                        if (robot.X == x && robot.Y == y)
                        {
                            Debug.Assert(!Field[y][x].HasWall);
                            //no robot in box
                            Debug.Assert(Boxes[y][x] == null);
                        }
                    }
                }
                
                var postMoveBoxes = Boxes.SelectMany(x => x).Where(x => x is not null).ToArray();
                for (int i = 0; i < postMoveBoxes.Length; i++)
                {
                    Debug.Assert( (Math.Abs(postMoveBoxes[i].X - boxCoords[i].X) == 1 && Math.Abs(postMoveBoxes[i].Y - boxCoords[i].Y) == 0) 
                               || (Math.Abs(postMoveBoxes[i].Y - boxCoords[i].Y) == 1 && Math.Abs(postMoveBoxes[i].X - boxCoords[i].X) == 0)
                               || (Math.Abs(postMoveBoxes[i].Y - boxCoords[i].Y) == 0 && Math.Abs(postMoveBoxes[i].X - boxCoords[i].X) == 0));
                }
                
            }
            Console.WriteLine(Boxes.SelectMany(x => x).Where(x => x != null)
                                   .Cast<BoxPart2>()
                                   .Select(x => x.GetGps())
                                   .Sum());
        }
       
    }
}