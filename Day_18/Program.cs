// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_18;

using Silk.NET.Maths;

class Program
{
    private static Tile[][] ParseMap(string map, int maxLines = 1024)
    {
        var ret = new Tile[71][];
        var lines = map.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (var i = 0; i < ret.Length; i++)
        {
            ret[i] = new Tile[71];
        }
        for (int x = 0; x < 71; x++)
        {
            for (int y = 0; y < 71; y++)
            {
                ret[x][y] = new Tile
                {
                    X = x,
                    Y = y,
                    HasWall = false
                };
            }
        }
        for (var i = 0; i < maxLines; i++)
        {
            var input = lines[i].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            ret[int.Parse(input[0])][int.Parse(input[1])].HasWall = true;
        }
        return ret;
    }
    
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }

    public static void PrintMap(Tile[][] map)
    {
        foreach (Tile[] tilese in map)
        {
            foreach (var tile in tilese)
            {
                if (tile.HasWall)
                    Console.Write('#');
                else
                    Console.Write('.');
            }
            Console.WriteLine();
        }
    }
    
    static async Task Main(string[] args)
    {
        var map = ParseMap(await ReadInputAsync());
        PrintMap(map);
        var d = new Dijkstras(Vector2D<int>.Zero, new Vector2D<int>(70, 70), map);
        var path = d.Run().ToArray();
        Console.WriteLine(path.Length);
    }
}