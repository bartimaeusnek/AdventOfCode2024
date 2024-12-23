// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_17;

class Program
{
    static void Main(string[] args)
    {
        var computer = new Computer
        {
            Register_A = 0,
            Register_B = 0,
            Register_C = 0,
            Instructions = []
        };
        computer.Execute();
        computer.PrintOutput();
    }
}