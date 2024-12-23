// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_17;

using System.Text;

public class Computer
{
    public long Register_A;
    public long Register_B;
    public long Register_C;

    public int InstructionPointer;
    public List<long> Instructions;
    public readonly List<long> Output = [];

    public void PrintOutput()
    {
        StringBuilder sb = new();
        foreach (long l in Output)
        {
            sb.Append(l);
            sb.Append(',');
        }
        sb.Length -= 1;
        Console.WriteLine(sb.ToString());
    }
    
    public long _kommas;
    public long KommasSet { get => _kommas -1; set => _kommas = value; }

    public void Execute()
    {
        while (true)
        {
            try
            {
                ((Instructions)Instructions[InstructionPointer]).Execute(this, new Operand(this, Instructions[InstructionPointer + 1]));
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
        }
    }
    
}

public enum Instructions
{
    adv = 0,
    bxl = 1,
    bst = 2,
    jnz = 3,
    bxc = 4,
    @out = 5,
    bdv = 6,
    cdv = 7
}

public class Operand(Computer computer, long literalValue)
{
    public readonly long LiteralValue = literalValue;
    public long ComboValue()
    {
        return LiteralValue switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => computer.Register_A,
            5 => computer.Register_B,
            6 => computer.Register_C,
            _ => throw new NotSupportedException()
        };
    }
}

public static class InstructionsExtensions
{
    public static void Execute(this Instructions instruction, Computer computer, Operand operant)
    {
        switch (instruction)
        {
            case Instructions.adv:
            {
                var divisor = (int) Math.Pow(2, operant.ComboValue());
                computer.Register_A /= divisor;
                break;
            }
            case Instructions.bxl:
            {
                computer.Register_B ^= operant.LiteralValue;
                break;
            }
            case Instructions.bst:
            {
                computer.Register_B = operant.ComboValue() % 8;
                break;
            }
            case Instructions.jnz:
            {
                if (computer.Register_A == 0)
                {
                    break;
                }
                computer.InstructionPointer = (int)operant.LiteralValue;
                return;
            }
            case Instructions.bxc:
            {
                computer.Register_B ^= computer.Register_C;
                break;
            }
            case Instructions.@out:
            {
                computer.Output.Add(operant.ComboValue() % 8);
                computer._kommas++;
                break;
            }
            case Instructions.bdv:
            {
                var divisor = (int) Math.Pow(2, operant.ComboValue());
                computer.Register_B = computer.Register_A / divisor;
                break;
            }
            case Instructions.cdv:
            {
                var divisor = (int) Math.Pow(2, operant.ComboValue());
                computer.Register_C = computer.Register_A / divisor;
                break;
            }
        }  
        computer.InstructionPointer++;
        computer.InstructionPointer++;
    }
}