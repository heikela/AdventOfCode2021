using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Day24
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
                        ModelValidator validator = new ModelValidator(File.ReadLines("../../../../input24.txt"));

                        var result = validator.FindValidModelNumber();
            */
            new ModelValidator2().Solve();
        }
    }

    class ModelValidator2
    {
        List<Transform> Transforms;
        public ModelValidator2()
        {
            Transforms = new List<Transform>()
            {
                new Transform(true, -10, 13),
                new Transform(true, -8, 13),
                new Transform(false, 11, 2),
                new Transform(false, 14, 12),
                new Transform(true, -4, 15),
                new Transform(true, -11, 15),
                new Transform(true, -7, 1),
                new Transform(false, 10, 15),
                new Transform(true, -9, 2),
                new Transform(false, 10, 14),
                new Transform(true, -3, 15),
                new Transform(false, 12, 2),
                new Transform(false, 11, 15),
                new Transform(false, 12, 7)
            };
        }

        public long Solve()
        {
            Dictionary<int, string> bestSuffixes = new Dictionary<int, string>();
            bestSuffixes.Add(0, "");
            foreach (Transform transform in Transforms)
            {
                Dictionary<int, string> newBestSuffixes = new Dictionary<int, string>();
                int searchMax = Math.Max(26, bestSuffixes.Keys.Max()) * 50;
                for (int zIn = 0; zIn < searchMax; ++zIn)
                {
                    for (int w = 1; w <= 9; ++w)
                    {
                        int zOut = transform.Apply(w, zIn);
                        if (bestSuffixes.ContainsKey(zOut))
                        {
                            newBestSuffixes.Add(zIn, w.ToString() + bestSuffixes[zOut]);
                            break;
                        }
                    }
                }
                bestSuffixes = newBestSuffixes;
            }
            Console.WriteLine($"{bestSuffixes[0]}");
            return long.Parse(bestSuffixes[0]);
/*
            for (int z = 0; z <= 10000; ++z)
            {
                for (int w13 = 1; w13 <= 9; ++w13)
                {
                    for (int w14 = 1; w14 <= 9; ++w14)
                    {
                        if (T14.Apply(w14, T13.Apply(w13, z)) == 0)
                        {
                            Console.WriteLine($"Arriving at the last two transformations with Z = {z} and inputs = {w13}, {w14} would work.");
                        }
                    }

                }
            }*/
        }
    }

    class ModelValidator {
        List<Instruction> Program;
        Dictionary<string, (int ip, Dictionary<string, int> registers)> CachedStates;

        public ModelValidator(IEnumerable<string> input)
        {
            Program = input.Select(Instruction.FromLine).ToList();
            CachedStates = new Dictionary<string, (int ip, Dictionary<string, int> registers)>();
        }

        public long FindValidModelNumber()
        {
            long number = 100000000000000;
            for (bool valid = false; !valid; --number)
            {
                List<int> digits = number.ToString().ToList().Select(c => int.Parse(c.ToString())).ToList();
                if (digits.Any(d => d == 0))
                {
                    continue;
                }
                var res = RunProgram(digits);
                if (res == 0)
                {
                    valid = true;
                    Console.WriteLine($"Number {number} is a valid model number");
                    return number;
                }
            }
            return -1;
        }

        int RunProgram(List<int> inputs)
        {
            Dictionary<string, int> state = new Dictionary<string, int>();
            state.Add("w", 0);
            state.Add("x", 0);
            state.Add("y", 0);
            state.Add("z", 0);
            int inputIndex = 0;
            int ip = 0;

            for (int prefixLength = inputs.Count(); prefixLength >= 0; --prefixLength)
            {
                string prefix = string.Join("", inputs.Take(prefixLength).Select(i => i.ToString()));
                if (prefix.Length != prefixLength)
                {
                    throw new Exception("Prefix does not have the expected length, there is a bug somewhere");
                }
                if (CachedStates.ContainsKey(prefix))
                {
                    ip = CachedStates[prefix].ip;
                    state = new Dictionary<string, int>(CachedStates[prefix].registers);
                    inputIndex = prefixLength;
                    break;
                }
            }

            for (; ip < Program.Count; ++ip)
            {
                Instruction inst = Program[ip];
                switch (inst.op)
                {
                    case "inp":
                        string prefix = string.Join("", inputs.Take(inputIndex).Select(i => i.ToString()));
                        if (!CachedStates.ContainsKey(prefix))
                        {
                            CachedStates.Add(prefix, (ip, state));
                        }
                        state[inst.a1] = inputs[inputIndex];
                        inputIndex++;
                        break;
                    case "add":
                        state[inst.a1] += inst.Arg2Value(state);
                        break;
                    case "mul":
                        state[inst.a1] *= inst.Arg2Value(state);
                        break;
                    case "div":
                        state[inst.a1] /= inst.Arg2Value(state);
                        break;
                    case "mod":
                        state[inst.a1] %= inst.Arg2Value(state);
                        break;
                    case "eql":
                        state[inst.a1] = state[inst.a1] == inst.Arg2Value(state) ? 1 : 0;
                        break;
                }
            }
            return state["z"];
        }
    }

    public record Instruction(string op, string a1, string a2 = null)
    {
        public static Instruction FromLine(string line)
        {
            var parts = line.Split(' ');
            if (parts.Count() > 2)
            {
                return new Instruction(parts[0], parts[1], parts[2]);
            }
            else
            {
                return new Instruction(parts[0], parts[1]);
            }
        }

        public int Arg2Value(Dictionary<string, int> registers)
        {
            if (a2 == "w" || a2 == "x" || a2 == "y" || a2 == "z")
            {
                return registers[a2];
            }
            else
            {
                return int.Parse(a2);
            }
        }
    }

    public record Transform(bool divz, int addToX, int addToY)
    {
        public int Apply(int w, int z)
        {
            int x = z % 26 + addToX;
            if (divz)
            {
                z = z / 26;
            }
            if (x != w)
            {
                x = 1;
            }
            else
            {
                x = 0;
            }
            z = z * (25 * x + 1);
            z = z + x * (w + addToY);
            return z;
        }
    }
}
