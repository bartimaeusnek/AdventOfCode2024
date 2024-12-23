// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_20;

using Silk.NET.Maths;

class Program
{
    public const string DebugInput = """
                                     ###############
                                     #...#...#.....#
                                     #.#.#.#.#.###.#
                                     #S#...#.#.#...#
                                     #######.#.#.###
                                     #######.#.#...#
                                     #######.#.###.#
                                     ###..E#...#...#
                                     ###.#######.###
                                     #...###...#...#
                                     #.#####.#.###.#
                                     #.#...#.#.#...#
                                     #.#.#.#.#.#.###
                                     #...#...#...###
                                     ###############
                                     """;
    
    private static Tile[][] ParseMap(string map, out Vector2D<int> start, out Vector2D<int> end)
    {
        start = Vector2D<int>.Zero;
        end = Vector2D<int>.Zero;
        var lines = map.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var ret = new Tile[lines.Length][];
        for (var i = 0; i < ret.Length; i++)
        {
            ret[i] = new Tile[lines[i].Length];
        }
        for (int y = 0; y < lines.Length; y++)
        {
            for (int x = 0; x < lines[y].Length; x++)
            {
                var current = lines[y][x];
                switch (current)
                {
                    case 'S':
                        start = new Vector2D<int>(x, y);
                        break;
                    case 'E':
                        end = new Vector2D<int>(x, y);
                        break;
                }
                ret[y][x] = new Tile
                {
                    X = x,
                    Y = y,
                    HasWall = current == '#'
                };
            }
        }
        return ret;
    }
    
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    
    static async Task Main(string[] args)
    {
        var input = await ReadInputAsync();
        var map = ParseMap(input, out var start, out var end);
        var originalLength = new Dijkstras(start, end, map).Run().Count();
        var wpf = new WallPathFinder(map, start, end);
        Console.WriteLine(wpf.FindPathsWithRemovedWalls().Count(x => originalLength - x.PathLength >= 100));
    }
}

public class WallPathFinder(Tile[][] map, Vector2D<int> start, Vector2D<int> end)
{
    public IEnumerable<(Vector2D<int> WallLocation, int PathLength)> FindPathsWithRemovedWalls()
    {
        for (int y = 0; y < map.Length; y++)
            for (int x = 0; x < map[y].Length; x++)
            {
                if (!map[y][x].HasWall)
                    continue;

                var newMap = CloneMap(map);
                newMap[y][x].HasWall = false;
                yield return (new Vector2D<int>(x, y), new Dijkstras(start, end, newMap).Run().Count());
            }
    }

    private static Tile[][] CloneMap(Tile[][] original)
    {
        var clone = new Tile[original.Length][];
        
        for (int y = 0; y < original.Length; y++)
        {
            clone[y] = new Tile[original[y].Length];
            for (int x = 0; x < original[y].Length; x++)
            {
                clone[y][x] = new Tile
                {
                    X = original[y][x].X,
                    Y = original[y][x].Y,
                    HasWall = original[y][x].HasWall
                };
            }
        }
        
        return clone;
    }
}