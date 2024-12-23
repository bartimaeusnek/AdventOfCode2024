// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_22;

class Program
{
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    
    static async Task Main(string[] args)
    {
        var input = await ReadInputAsync();
        var inputs = input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(uint.Parse).ToArray();

        for (int index = 0; index < inputs.Length; index++)
        {
            PartOne(ref inputs[index], 2000);
        }
        Console.WriteLine(inputs.Sum(x => x));
      
        // uint number1 = 1;
        // uint number2 = 10;
        // uint number3 = 100;
        // uint number4 = 2024;
        // PartOne(ref number1, 2000);
        // PartOne(ref number2, 2000);
        // PartOne(ref number3, 2000);
        // PartOne(ref number4, 2000);
        //
        // Console.WriteLine(number1 + number2 + number3 + number4);
    }

    public static void PartOne(ref uint secretNumber, uint iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            Mix(ref secretNumber, secretNumber * 64);
            Prune(ref secretNumber);
            Mix(ref secretNumber, secretNumber / 32);
            Prune(ref secretNumber);
            Mix(ref secretNumber, secretNumber * 2048);
            Prune(ref secretNumber);
        }
    }
    
    public static void Mix(ref uint secretNumber, uint operand)
    {
        secretNumber ^= operand;
    }
    
    public static void Prune(ref uint secretNumber)
    {
        secretNumber %= 16777216;
    }
}