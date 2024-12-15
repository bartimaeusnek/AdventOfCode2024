// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_7;

public enum Operations
{
    Add = 0,
    Multiply = 1,
    Concat = 2
}

public class Equasion
{
    private readonly ulong _result;
    private readonly Queue<ulong> _numbers = new Queue<ulong>();
    private readonly Queue<Operations> _operationQueue = new Queue<Operations>();
    private ulong _intermediateResult;
    public Equasion(ulong result, IEnumerable<ulong> numbers)
    {
        _result = result;
        foreach (ulong number in numbers)
        {
            _numbers.Enqueue(number);
        }
    }
    public Equasion(ulong result, Queue<ulong> numbers)
    {
        _result = result;
        _numbers = numbers;
    }
    private static readonly List<Operations> PossibleOperationsPart1 = [Operations.Add, Operations.Multiply];
    private static readonly List<Operations> PossibleOperationsPart2 = [Operations.Add, Operations.Multiply, Operations.Concat];
    public ulong Result => _result;

    public bool SolveAll(bool part2)
    {
        var operationCount = _numbers.Count - 1;
        var possibleOperationsForPart = part2 ? PossibleOperationsPart2 : PossibleOperationsPart1;
        var possibleOperationPermutations = (int)Math.Pow(possibleOperationsForPart.Count, operationCount);

        for (var i = 0; i < possibleOperationPermutations; i++)
        {
            var currentEquation = new Equasion(_result, new Queue<ulong>(_numbers));
            var tempIndex = i;

            for (int op = 0; op < operationCount; op++)
            {
                var operationIndex = tempIndex % possibleOperationsForPart.Count;
                var operation = possibleOperationsForPart[operationIndex];
                currentEquation._operationQueue.Enqueue(operation);

                tempIndex /= possibleOperationsForPart.Count;
            }

            if (currentEquation.Solve())
                return true;
        }
        return false;
    }

    private bool Solve()
    {
        _intermediateResult = _numbers.Dequeue();
        foreach (var operationse in _operationQueue)
        {
            var num2 = _numbers.Dequeue();
            _intermediateResult = operationse switch
            {
                Operations.Add => _intermediateResult + num2,
                Operations.Multiply => _intermediateResult * num2,
                Operations.Concat => _intermediateResult * (ulong)Math.Pow(10, (int)Math.Floor(Math.Log10(num2)) + 1) + num2,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        return _result == _intermediateResult;
    }
}