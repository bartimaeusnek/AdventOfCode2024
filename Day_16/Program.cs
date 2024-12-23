// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_16;

using Silk.NET.Maths;

class Program
{
    private static string DebugInput = """
                                       ###############
                                       #.......#....E#
                                       #.#.###.#.###.#
                                       #.....#.#...#.#
                                       #.###.#####.#.#
                                       #.#.#.......#.#
                                       #.#.#####.###.#
                                       #...........#.#
                                       ###.#.#####.#.#
                                       #...#.....#.#.#
                                       #.#.#.###.#.#.#
                                       #.....#...#.#.#
                                       #.###.#.#.#.#.#
                                       #S..#.....#...#
                                       ###############
                                       """;

    public static string DebugInput2 = """
                                       #################
                                       #...#...#...#..E#
                                       #.#.#.#.#.#.#.#.#
                                       #.#.#.#...#...#.#
                                       #.#.#.#.###.#.#.#
                                       #...#.#.#.....#.#
                                       #.#.#.#.#.#####.#
                                       #.#...#.#.#.....#
                                       #.#.#####.#.###.#
                                       #.#.#.......#...#
                                       #.#.###.#####.###
                                       #.#.#...#.....#.#
                                       #.#.#.#####.###.#
                                       #.#.#.........#.#
                                       #.#.#.#########.#
                                       #S#.............#
                                       #################
                                       """;
    
    
    public static Tile[][] Board;
    public static Vector2D<int> End;
    public static Vector2D<int> Start;
    public static void LoadBoard(string input)
    {
        var lines = input.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Board = new Tile[lines.Length][];
        for (int y = 0; y < lines.Length; y++)
        {
            Board[y] = new Tile[lines.Length];
            for (int x = 0; x < lines[y].Length; x++)
            {
                Board[y][x] = new Tile(lines[y][x] == '#');
                if (lines[y][x] == 'E')
                    End = new Vector2D<int>(x, y);
                if (lines[y][x] == 'S')
                    Start = new Vector2D<int>(x, y);
            }
        }
    }

    public static void PrintBoard(Vector3D<int>[] path)
    {
        for (int y = 0; y < Board.Length; y++)
        {
            for (int x = 0; x < Board[y].Length; x++)
            {
                if (y == End.Y && x == End.X)
                    Console.Write('E');
                if (y == Start.Y && x == Start.X)
                    Console.Write('S');
                if (path.Any(z => z.X == x && z.Y == y))
                    Console.Write('X');
                else
                    Console.Write(Board[y][x].HasWall ? '#' : '.');
            }
            Console.WriteLine();
        }
    }
    
    static async Task Main(string[] args)
    {
        using (var file = File.OpenText("input"))
        {
            LoadBoard(await file.ReadToEndAsync());
        }
        var d = new Dijkstras3D(Start, End, Board);
        var positions = d.Run(Directions.EAST).ToArray();
        // for (var i = 0; i < positions.Length; i++)
        // {
            // Console.Clear(); 
            // Console.WriteLine(positions[i]);
            // Console.WriteLine(d.CostSoFar[positions[i]]);
            // Console.ReadKey();
        // }
        
        PrintBoard(positions);
        Console.WriteLine(positions.Length);
        Console.WriteLine(d.CostSoFar[d.Last]);
    }
}
public record struct Tile(bool HasWall);

public enum Directions
{
    NORTH = 1,
    EAST = 2,
    SOUTH = 3,
    WEST = 4,
}