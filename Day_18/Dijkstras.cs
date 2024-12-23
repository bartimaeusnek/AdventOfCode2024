// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_18;

using Silk.NET.Maths;

public class Dijkstras(
    Vector2D<int> from, 
    Vector2D<int> to, 
    Tile[][] map)
{
    public Vector2D<int> From => from;
    public Vector2D<int> To => to;
    
    public readonly Dictionary<Vector2D<int>, Vector2D<int>> CameFrom = new ();
    public readonly Dictionary<Vector2D<int>, int> CostSoFar = new ();
    public readonly PriorityQueue<Vector2D<int>, int> Frontier = new ();

    public int Turns = 0;
    
    public IEnumerable<Vector2D<int>> Run()
    {
        Frontier.Enqueue(From, 0);
        CostSoFar[From] = 0;
        
        while (Frontier.Count > 0)
        {
            var current = Frontier.Dequeue();
            if (To == current)
            {
                return GetPath().Reverse().Skip(1);
            }
            for (var y = -1; y <= 1; y++)
            {
                for (var x = -1; x <= 1; x++)
                {
                    if (x != 0 && y != 0)
                        continue;
                    if (x == 0 && y == 0)
                        continue;
                    
                    var nextPos = current + new Vector2D<int>(x, y);
                    Tile terrain;
                    if (nextPos.Y >= 0 && nextPos.Y < map.Length)
                    {
                        if (nextPos.X >= 0 && nextPos.X < map[nextPos.Y].Length)
                        {
                            terrain = map[nextPos.Y][nextPos.X];
                        }
                        else continue;
                    }
                    else continue;
                    
                    if (terrain.HasWall)
                        continue;

                    int totalCost = CostSoFar[current] + 1;

                    if (CostSoFar.TryGetValue(nextPos, out int value) && totalCost >= value)
                        continue;

                    value = totalCost;
                    CostSoFar[nextPos] = value;
                    Frontier.Enqueue(nextPos, totalCost);
                    CameFrom[nextPos] = current;
                }
            }
        }

        return GetPath().Reverse().Skip(1);
    }
    
    private IEnumerable<Vector2D<int>> GetPath()
    {
        var curr = To;
        var start = From;
        yield return curr;
        while (curr != start)
        {
            var next = CameFrom[curr];
            yield return next;
            curr = next;
        }
    }
}