// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_23;

using System.Text;

class Program
{
    private static async Task<string> ReadInputAsync()
    {
        using var reader = new StreamReader("input");
        return await reader.ReadToEndAsync();
    }
    private const string DebugString = """
                                       kh-tc
                                       qp-kh
                                       de-cg
                                       ka-co
                                       yn-aq
                                       qp-ub
                                       cg-tb
                                       vc-aq
                                       tb-ka
                                       wh-tc
                                       yn-cg
                                       kh-ub
                                       ta-co
                                       de-co
                                       tc-td
                                       tb-wq
                                       wh-td
                                       ta-ka
                                       td-qp
                                       aq-cg
                                       wq-ub
                                       ub-vc
                                       de-ta
                                       wq-aq
                                       wq-vc
                                       wh-yn
                                       ka-de
                                       kh-ta
                                       co-tc
                                       wh-qp
                                       tb-vc
                                       td-yn
                                       """;
    
    static async Task Main(string[] args)
    {
        var input = await ReadInputAsync();
        var computers = ParseComputers(input);
        MatchComputers(input, computers);
        var q = new Queue<TriConnection>();
        foreach (var A in computers)
        {
            var aConnection = new TriConnection(A);
            foreach (var B in A.Connections)
            {
                if (B == A)
                    continue;
                foreach (var C in B.Connections)
                {
                    if (C == A || C == B)
                        continue;
                    foreach (var mayBeA in C.Connections)
                    {
                        if (mayBeA != A)
                            continue;
                        if (A.IsValid || B.IsValid || C.IsValid)
                        {
                            aConnection.B.Add(B);
                            if (!aConnection.C.TryGetValue(B, out var lst))
                            {
                                aConnection.C[B] = [C];
                            }
                            else
                            {
                                lst.Add(C);
                            }
                        }
                    }
                }
            }
            if (aConnection.C.Any())
                q.Enqueue(aConnection);
        }
        
        var computersSet = new HashSet<Connect>();
        foreach (var triConnection in q)
        {
            foreach ((var key, List<Computer>? value) in triConnection.C)
            {
                foreach (var computer in value)
                {
                    computersSet.Add(new Connect(new HashSet<Computer>
                    {
                        triConnection.A,
                        key,
                        computer
                    }));
                }
            }
        }
        Console.WriteLine(computersSet.Count);
    }

    static void MatchComputers(string input, Computer[] computers)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (string line in lines)
        {
            foreach (var computer in computers)
            {
                if (line[..2] == computer.StringRepresentation())
                {
                    computer.Connections.Add(computers.First(x => x.StringRepresentation() == line[3..]));
                }
                if (line[3..] == computer.StringRepresentation())
                {
                    computer.Connections.Add(computers.First(x => x.StringRepresentation() == line[..2]));
                }
            }
        }
    }
    
    static Computer[] ParseComputers(string input)
    {
        return input.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                            .SelectMany(x => x.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                              .Select(y => y.Trim('-')))
                            .Select(x =>
                            {
                                var idAsStrings = x.ToCharArray();
                                int idAsInt = idAsStrings[0] << 16 | idAsStrings[1];
                                return idAsInt;
                            })
                            .Distinct()
                            .Select(x => new Computer(x))
                            .ToArray();
        
    }
}

public record struct Connect(HashSet<Computer> Hs)
{
    public readonly bool Equals(Connect other)
    {
        return Hs.Aggregate(true, (current, computer) => current & other.Hs.Contains(computer));
    }
    public override readonly int GetHashCode()
    {
        return Hs.Aggregate(0, (resulting, computer) => resulting + computer.Id);
    }
    public override string ToString()
    {
        return Hs.Aggregate("", (c, computer) => c + " " + computer);
    }
}

public record Computer(int Id)
{
    public readonly int Id = Id;
    public HashSet<Computer> Connections = new HashSet<Computer>();

    public bool IsValid => (char)((Id >> 16) & 0xFFFF) == 't';
    
    public string StringRepresentation()
    {
        var sb = new StringBuilder(2);
        sb.Append((char) ((Id >> 16) & 0xFFFF));
        sb.Append((char) (Id & 0xFFFF));
        return sb.ToString();
    }
    
    public override string ToString()
    {
        return StringRepresentation();
    }
    
    public string ToDebugString()
    {
        var sb = new StringBuilder();
        sb.Append(StringRepresentation());
        sb.Append(" is connected to:");
        sb.AppendLine();
        foreach (var connection in Connections)
        {
            sb.AppendLine(connection.StringRepresentation());
        }
        
        return sb.ToString();
    }
}

public record TriConnection(Computer a)
{
    public Computer A = a;
    public List<Computer> B = new List<Computer>();
    public Dictionary<Computer, List<Computer>> C = new Dictionary<Computer, List<Computer>>();
}