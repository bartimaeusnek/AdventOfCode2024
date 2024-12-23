namespace Day_16;

using Silk.NET.Maths;

public class Dijkstras3D(
    Vector2D<int> from, 
    Vector2D<int> to, 
    Tile[][] map)
{
    public Vector2D<int> From => from;
    public Vector2D<int> To => to;
    
    public readonly Dictionary<Vector3D<int>, Vector3D<int>> CameFrom = new ();
    public readonly Dictionary<Vector3D<int>, int> CostSoFar = new ();
    public readonly PriorityQueue<Vector3D<int>, int> Frontier = new ();

    public int Turns = 0;
    public Vector3D<int> Last = Vector3D<int>.Zero;
    protected virtual int DirectionDistanceCost(Vector3D<int> currentPos, Vector3D<int> nextPos)
    {
        return (Directions)currentPos.Z switch
        {
            Directions.NORTH => (Directions)nextPos.Z switch
            {
                Directions.SOUTH => 2000,
                _ => 1000
            },
            Directions.EAST => (Directions)nextPos.Z switch
            {
                Directions.WEST => 2000,
                _ => 1000
            },
            Directions.SOUTH => (Directions)nextPos.Z switch
            {
                Directions.NORTH => 2000,
                _ => 1000
            },
            Directions.WEST => (Directions)nextPos.Z switch
            {
                Directions.EAST => 2000,
                _ => 1000
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public IEnumerable<Vector3D<int>> Run(Directions direction)
    {
        var start = new Vector3D<int>(From, (int)direction);
        Frontier.Enqueue(start, 0);
        CostSoFar[start] = 0;
        
        while (Frontier.Count > 0)
        {
            var current = Frontier.Dequeue();
            if (To == new Vector2D<int>(current.X, current.Y))
            {
                Last = current;
                return GetPath(direction).Reverse().Skip(1);
            }
            for (var y = -1; y <= 1; y++)
            {
                for (var x = -1; x <= 1; x++)
                {
                    if (x != 0 && y != 0)
                        continue;
                    int totalCost;
                    int value;
                    var nextPos = current + new Vector3D<int>(x, y, 0);
                    if (x == 0 && y == 0)
                    {
                        for (int i = 1; i < 5; i++)
                        {
                            if (i == current.Z)
                                continue;
                            
                            nextPos.Z = i;
                            totalCost = CostSoFar[current] + DirectionDistanceCost(current, nextPos);
                    
                            if (CostSoFar.TryGetValue(nextPos, out value) && totalCost >= value)
                                continue;

                            value = totalCost;
                            CostSoFar[nextPos] = value;
                            Frontier.Enqueue(nextPos, totalCost);
                            CameFrom[nextPos] = current;
                        }
                        continue;
                    }
                    
                    switch (current.Z)
                    {
                        case 1:
                            if (y != -1)
                                continue;
                            break;
                        case 2:
                            if (x != 1)
                                continue;
                            break;
                        case 3:
                            if (y != 1)
                                continue;
                            break;
                        case 4:
                            if (x != -1)
                                continue;
                            break;
                    }
                    
                    
                    Tile terrain;
                    if (nextPos.Y > 0 && nextPos.Y < map.Length)
                    {
                        if (nextPos.X > 0 && nextPos.X < map[nextPos.Y].Length)
                        {
                            terrain = map[nextPos.Y][nextPos.X];
                        }
                        else continue;
                    }
                    else continue;
                    
                    if (terrain.HasWall)
                        continue;

                    totalCost = CostSoFar[current] + 1;
                    
                    if (CostSoFar.TryGetValue(nextPos, out value) && totalCost >= value)
                        continue;

                    value = totalCost;
                    CostSoFar[nextPos] = value;
                    Frontier.Enqueue(nextPos, totalCost);
                    CameFrom[nextPos] = current;
                }
            }
        }

        return GetPath(direction).Reverse().Skip(1);
    }
    
    private IEnumerable<Vector3D<int>> GetPath(Directions startDircection)
    {
        var curr = Last;
        var start = new Vector3D<int>(From, (int)startDircection);
        yield return curr;
        while (curr != start)
        {
            var next = CameFrom[curr];
            yield return next;
            curr = next;
        }
    }
}