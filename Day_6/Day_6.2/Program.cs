namespace Day_6._2;

class Program
{
    private static readonly string debugInput =
        """
        ....#.....
        .........#
        ..........
        ..#.......
        .......#..
        ..........
        .#..^.....
        ........#.
        #.........
        ......#...
        """;
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    
    static async Task Main(string[] args)
    {
        var input = await ReadInputAsync();
        var board = BuildLists(input);
        var solved = Solve(board, out _);
        Console.WriteLine(solved.Count);
        var modifiedBoards = BuildModifiedLists(board, solved).ToArray();

        var solutions = 0;
        Parallel.ForEach(modifiedBoards, board =>
        {
            Solve(board, out var isInfinite);
            if (isInfinite)
                Interlocked.Increment(ref solutions);
        });
        Console.WriteLine(solutions);
    }
    
    private static IEnumerable<Tile[][]> BuildModifiedLists(Tile[][] board, HashSet<(Tile, Direction)> path)
    {
        var flattened = board.SelectMany(x => x).ToArray();
        var maxX = flattened.Max(x => x.X);
        var maxY = flattened.Max(x => x.Y);
        for (int y = 0; y <= maxY; y++)
        {
            for (int x = 0; x <= maxX; x++)
            {
                if (!path.Any(z => z.Item1.X == x && z.Item1.Y == y))
                    continue;
                var ret = new Tile[maxY+1][];
                for (int cy = 0; cy <= maxY; cy++)
                {
                    ret[cy] = new Tile[maxX+1];
                    for (int cx = 0; cx <= maxX; cx++)
                    {
                        if (board[cy][cx].IsStartingPoint)
                        {
                            ret[cy][cx] = new Tile(board[cy][cx].X, board[cy][cx].Y, false, true);
                        }
                        else if (cy == y && cx == x)
                        {
                            ret[cy][cx] = new Tile(board[cy][cx].X, board[cy][cx].Y, true, false);
                        }
                        else
                        {
                            ret[cy][cx] = new Tile(board[cy][cx].X, board[cy][cx].Y, board[cy][cx].IsWall, false);
                        }
                      
                    }
                }
                yield return ret;
            }
        }
    }
    
    
    private record struct Tile(int X, int Y, bool IsWall, bool IsStartingPoint);
    private static Tile[][] BuildLists(string readInput)
    {
        var lines = readInput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var ret = new Tile[lines.Length][];
        for (int index = 0; index < lines.Length; index++)
        {
            string line = lines[index];
            ret[index] = new Tile[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                ret[index][i] = new Tile(i, index, c == '#', c == '^');
            }
        }
        return ret;
    }

    enum Direction
    {
        North,
        South,
        East,
        West,
    }
    
    private static HashSet<(Tile, Direction)> Solve(Tile[][] board, out bool isInfinite)
    {
        var flattened = board.SelectMany(x => x).ToArray();
        var startingPoint = flattened.First(x => x.IsStartingPoint);
        var maxX = flattened.Max(x => x.X);
        var maxY = flattened.Max(x => x.Y);
        var visited = new HashSet<(Tile, Direction)>();
        var currentDirection = Direction.North;
        Tile? currentTile = startingPoint;
        visited.Add((startingPoint, currentDirection));
        while (currentTile != null)
        {
            start:
            (int X, int Y) nextCoord = currentDirection switch
            {
                Direction.North => (currentTile.Value.X, currentTile.Value.Y - 1),
                Direction.South => (currentTile.Value.X, currentTile.Value.Y + 1),
                Direction.East => (currentTile.Value.X + 1, currentTile.Value.Y),
                Direction.West => (currentTile.Value.X - 1, currentTile.Value.Y),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (nextCoord.Y >= 0 && nextCoord.Y <= maxY)
            {
                if (nextCoord.X >= 0 && nextCoord.X <= maxX)
                {
                    var nextTile = board[nextCoord.Y][nextCoord.X];
                    if (nextTile.IsWall)
                    {
                        currentDirection = currentDirection switch
                        {
                            Direction.North => Direction.East,
                            Direction.South => Direction.West,
                            Direction.East => Direction.South,
                            Direction.West => Direction.North,
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        goto start;
                    }
                    currentTile = nextTile;
                    if (!visited.Add((currentTile.Value, currentDirection)))
                    {
                        isInfinite = true;
                        return visited;
                    }
                }
                else
                    currentTile = null;
            }
            else
                currentTile = null;
        }
        isInfinite = false;
        return visited;
    }

  
}