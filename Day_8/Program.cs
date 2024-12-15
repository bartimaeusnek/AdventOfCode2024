// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_8;

class Program
{
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    private static void BuildLists(string readInput)
    {
        var lines = readInput.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var ret = new Cell[lines.Length][];
        for (int index = 0; index < lines.Length; index++)
        {
            string line = lines[index];
            ret[index] = new Cell[line.Length];
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                ret[index][i] = new Cell(i, index, c);
            }
        }
        Cell.Field = ret;
    }
    
    static async Task Main(string[] args)
    {
        BuildLists(await ReadInputAsync());
        for (int i = 0; i < Cell.Field.Length; i++)
        {
            Cell[] cellse = Cell.Field[i];
            for (int index = 0; index < cellse.Length; index++)
            {
                var cell = cellse[index];
                cell.BroadcastAntinodes(Cell.Field.Length, cellse.Length);
            }
        }
        Console.WriteLine(Cell.CountAntinodes());
        
        BuildLists(await ReadInputAsync());
        for (int i = 0; i < Cell.Field.Length; i++)
        {
            Cell[] cellse = Cell.Field[i];
            for (int index = 0; index < cellse.Length; index++)
            {
                var cell = cellse[index];
                cell.BroadcastAntinodes2(Cell.Field.Length, cellse.Length);
            }
        }
        Console.WriteLine(Cell.CountAntinodes());
    }
}

class Cell
{
    public static Cell[][] Field;
    public static readonly HashSet<char> AntennaTypes = new HashSet<char>();
    
    public Cell(int x, int y, char antennaType)
    {
        _antennaType = antennaType;
        _ = AntennaTypes.Add(antennaType);
        X = x;
        Y = y;
    }
    
   
    private char _antennaType;
    private HashSet<char> _antiNodeTypes = new HashSet<char>();
    public readonly int X;
    public readonly int Y;
    public bool IsEmpty => AntennaType == '.';
    public bool IsAntinode
    {
        get => _antennaType == '#';
        set
        {
            if (!value)
                return;

            _antennaType = '#';
        }
    }
    public char AntennaType
    {
        get => _antennaType == '#' ? '.' : _antennaType;
        set => _antennaType = value;
    }

    public static int CountAntinodes() => Field.SelectMany(x => x).Count(x => x._antiNodeTypes.Count != 0);

    public static void PrintField()
    {
        for (var y = 0; y < Field.Length; y++)
        {
            for (var x = 0; x < Field[y].Length; x++)
            {
                Console.Write(Field[y][x]._antennaType);
            }
            Console.WriteLine();
        }
    }

    public void BroadcastAntinodes(int maxY, int maxX)
    {
        if (AntennaType == '.')
            return;

        var cx = -X;
        var cy = -Y;
        var maxCx = maxX - X;
        var maxCy = maxY - Y;

        for (int dy = cy; dy < maxCy; dy++)
        {
            for (int dx = cx; dx < maxCx; dx++)
            {
                if (Field[Y + dy][X + dx].AntennaType != AntennaType)
                    continue;

                var targetPositionY = Y + dy + dy;
                var targetPositionX = X + dx + dx;

                if (targetPositionY == Y && targetPositionX == X)
                    continue;

                if (targetPositionY >= maxY || targetPositionY < 0
                                            || targetPositionX >= maxX 
                                            || targetPositionX < 0
                                            || !Field[targetPositionY][targetPositionX].IsEmpty)
                    continue;
                
                Field[targetPositionY][targetPositionX].IsAntinode = true;
                _ = Field[targetPositionY][targetPositionX]._antiNodeTypes.Add(AntennaType);
            }
        }
    }
    public void BroadcastAntinodes2(int maxY, int maxX)
    {
        if (AntennaType == '.')
            return;

        var cx = -maxX;
        var cy = -maxY;
        var maxCx = maxX;
        var maxCy = maxY;

        var upFunc = (int a, int b) => a - b;
        var sameFunc = (int a, int b) => a;
        var downFunc = (int a, int b) => a + b;
        
        for (int dy = cy; dy < maxCy; dy++)
        {
            for (int dx = cx; dx < maxCx; dx++)
            {
                BroadCastLogic2Internal(dx, dy, maxX, maxY, upFunc, sameFunc);   // ↑
                BroadCastLogic2Internal(dx, dy, maxX, maxY, upFunc, downFunc);   // ↗
                BroadCastLogic2Internal(dx, dy, maxX, maxY, sameFunc, downFunc); // →
                BroadCastLogic2Internal(dx, dy, maxX, maxY, downFunc, downFunc); // ↘
                BroadCastLogic2Internal(dx, dy, maxX, maxY, downFunc, sameFunc); // ↓
                BroadCastLogic2Internal(dx, dy, maxX, maxY, downFunc, upFunc);   // ↙
                BroadCastLogic2Internal(dx, dy, maxX, maxY, sameFunc, upFunc);   // ←
                BroadCastLogic2Internal(dx, dy, maxX, maxY, upFunc, upFunc);     // ↖
                //sameFunc sameFunc doesnt make sense here.
            }
        }
    }
    
    private void BroadCastLogic2Internal(int dx, int dy, int maxX, int maxY, Func<int, int, int> getTargetPositionY, Func<int, int, int> getTargetPositionX)
    {
        var targetPositionY = getTargetPositionY(Y, dy);
        var targetPositionX = getTargetPositionX(X, dx);
        
        if (!CheckBounds() || Field[targetPositionY][targetPositionX].AntennaType != AntennaType)
            return;

        while (Field[targetPositionY][targetPositionX].AntennaType == AntennaType 
            || Field[targetPositionY][targetPositionX]._antiNodeTypes.Contains(AntennaType))
        {
            targetPositionY = getTargetPositionY(targetPositionY, dy);
            targetPositionX = getTargetPositionX(targetPositionX, dx);

            _ = _antiNodeTypes.Add(AntennaType);
            if (targetPositionY == Y && targetPositionX == X)
                break;

            if (!CheckBounds())
                break;

            if (Field[targetPositionY][targetPositionX].IsEmpty)
                Field[targetPositionY][targetPositionX].IsAntinode = true;

            _ = Field[targetPositionY][targetPositionX]._antiNodeTypes.Add(AntennaType);
        }
        return;

        bool CheckBounds() => targetPositionY < maxY && targetPositionY >= 0 && targetPositionX < maxX && targetPositionX >= 0;
    }
}