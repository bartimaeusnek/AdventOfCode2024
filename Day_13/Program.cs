// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace Day_13;
using Silk.NET.Maths;

internal static class Program
{
    private const string DebugInput = """
                                Button A: X+94, Y+34
                                Button B: X+22, Y+67
                                Prize: X=8400, Y=5400

                                Button A: X+26, Y+66
                                Button B: X+67, Y+21
                                Prize: X=12748, Y=12176

                                Button A: X+17, Y+86
                                Button B: X+84, Y+37
                                Prize: X=7870, Y=6450

                                Button A: X+69, Y+23
                                Button B: X+27, Y+71
                                Prize: X=18641, Y=10279
                                """;
    
    private const long ButtonACost = 3;
    private const long ButtonBCost = 1;
    
    public record struct ButtonConfig(int MovementX, int MovementY);
    public record struct PrizePosition(long X, long Y);
    public record struct ClawConfig(ButtonConfig A, ButtonConfig B, PrizePosition Prize);
    private static async Task<ICollection<ClawConfig>> ReadConfig(TextReader fred)
    {
        const int ButtonA = 0;
        const int ButtonB = 1;
        const int PrizePosition = 2;
        const int EmptyLine = 3;
        var ret = new List<ClawConfig>();
        string?[] rawClawConfig = new string?[4];
        do
        {
            int pos = 0;
            await foreach (string? se in GetNextBlock(fred))
            {
                rawClawConfig[pos++] = se;
            }
            ret.Add(new ClawConfig(ReadButtonConfig(ButtonA), ReadButtonConfig(ButtonB), ReadPrizePosition()));
        } while (rawClawConfig[EmptyLine] != null);
        
        return ret;
        
        ButtonConfig ReadButtonConfig(int button)
        {
            var stringToOperateOn = rawClawConfig[button]!;
            var aXmovementOffset = "Button A: X".Length;
            int aXDigits = 1;
            while (char.IsDigit(stringToOperateOn[aXmovementOffset..][aXDigits++])){}
            int aXMovement = int.Parse(stringToOperateOn[aXmovementOffset..(aXmovementOffset + aXDigits -1)]);
            var aYmovementOffset = aXmovementOffset + aXDigits + 3;
            int aYMovement = int.Parse(stringToOperateOn[aYmovementOffset..]);
            return new ButtonConfig(aXMovement, aYMovement);
        }

        PrizePosition ReadPrizePosition()
        {
            var aXmovementOffset = "Prize: X=".Length;
            int aXDigits = 0;
            while (char.IsDigit(rawClawConfig[PrizePosition]![aXmovementOffset..][aXDigits++])){}
            int prizeX = int.Parse(rawClawConfig[PrizePosition]![aXmovementOffset..(aXmovementOffset + aXDigits -1)]);
            var aYmovementOffset = aXmovementOffset + aXDigits + 3;
            int prizeY = int.Parse(rawClawConfig[PrizePosition]![aYmovementOffset..]);
            return new PrizePosition(prizeX, prizeY);
        }
    }
    private static async IAsyncEnumerable<string?> GetNextBlock(TextReader fred)
    {
        yield return await fred.ReadLineAsync();
        yield return await fred.ReadLineAsync();
        yield return await fred.ReadLineAsync();
        yield return await fred.ReadLineAsync();
    }

    private static async Task Main(string[] args)
    {
        using var fred = File.OpenText("input");
        // using var fred = new StringReader(DebugInput);
        var cfg = await ReadConfig(fred);
        var prizeCosts = new List<long>();
        foreach (var (aCfg, bCfg, prizePosition) in cfg)
        {
            var total = new List<long>();
            for (int aPresses = 1; aPresses < 101; aPresses++)
            {
                for (int bPresses = 1; bPresses < 101; bPresses++)
                {
                    var currentPositionX = bPresses * bCfg.MovementX + aPresses * aCfg.MovementX;
                    var currentPositionY = bPresses * bCfg.MovementY + aPresses * aCfg.MovementY;
                    var cost = aPresses * ButtonACost + bPresses * ButtonBCost;
                    if (currentPositionY == prizePosition.Y && currentPositionX == prizePosition.X)
                    {
                        total.Add(cost);
                    }
                }
            }
            var minCosts = total
                           .OrderBy(x => x)
                           .FirstOrDefault();
            if (minCosts is not 0)
            {
                prizeCosts.Add(minCosts);
            }
        }
        Console.WriteLine(prizeCosts.Sum());

        ulong totalCostPart2 = 0;
        var cfgPart2 = new List<ClawConfig>();
        foreach (var c in cfg)
        {
            cfgPart2.Add(c with { Prize = new PrizePosition(c.Prize.X + 10000000000000, c.Prize.Y + 10000000000000) });
        }
        
        foreach (var clawConfig in cfgPart2)
        {
            var goal = new Vector2D<long>(clawConfig.Prize.X, clawConfig.Prize.Y);

            var aButtonPressesAsVector = new Vector2D<long>(clawConfig.A.MovementX, clawConfig.A.MovementY);
            var bButtonPressesAsVector = new Vector2D<long>(clawConfig.B.MovementX, clawConfig.B.MovementY);
            
            var originalMatrix = new Matrix2X2<long>(aButtonPressesAsVector, bButtonPressesAsVector);
            var matrixG1 = new Matrix2X2<long>(goal, bButtonPressesAsVector);
            var matrixG2 = new Matrix2X2<long>(aButtonPressesAsVector, goal);
            
            var oPresses = originalMatrix.GetDeterminant();
            var aPresses = matrixG1.GetDeterminant();
            var bPresses = matrixG2.GetDeterminant();

            aPresses /= oPresses;
            bPresses /= oPresses;
            
            if (aPresses >= 0 && bPresses >= 0 
                                 //im not quite sure *why* i need those statements, but without them it doesnt work =)
                              && aPresses * clawConfig.A.MovementX + bPresses * clawConfig.B.MovementX == goal.X 
                              && aPresses * clawConfig.A.MovementY + bPresses * clawConfig.B.MovementY == goal.Y)
                totalCostPart2 += (ulong)(aPresses * ButtonACost + bPresses * ButtonBCost);
        }
        Console.WriteLine(totalCostPart2);
    }
}