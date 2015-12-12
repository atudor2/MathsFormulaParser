using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Alistair.Tudor.ConsoleUtil;
using Alistair.Tudor.MathsFormulaParser;
using Alistair.Tudor.MathsFormulaParser.Exceptions;
using Alistair.Tudor.MathsFormulaParser.Helpers.Extensions;

namespace TesterFrontend
{
    // NB: Dirty code ahead!
    class Program
    {
        private static FormulaOptimisationLevel _optimisationLevel;
        static void Main(string[] args)
        {
            Console.WriteLine($"Using Parser Version: { typeof(IFormulaEvaluator).Assembly.GetName().Version }");
            Console.WriteLine();
            if (ConsoleHelper.AskConsoleQuestion("Enable Formula Optimisation? "))
            {
                // To what level?
                var items = Enum.GetNames(typeof(FormulaOptimisationLevel));
                foreach (var t in items.Select((x, i) => new Tuple<string, int>(x, i)))
                {
                    Console.WriteLine($"{t.Item2 + 1} - {t.Item1}");
                }

                var count = items.Length;

                ConsoleHelper.AskConsoleQuestionCustomVerifier("What level? ", s =>
                {
                    int x;
                    if (!int.TryParse(s, out x))
                    {
                        // Bad int
                        return false;
                    }
                    return (x >= 1) && (x <= count);
                }, s =>
                {
                    _optimisationLevel =
                        (FormulaOptimisationLevel)Enum.Parse(typeof(FormulaOptimisationLevel), items[int.Parse(s) - 1]);
                    return true;
                });
            }
            else
            {
                _optimisationLevel = FormulaOptimisationLevel.None;
            }

            Console.WriteLine();
            Console.WriteLine();
            var stopWatch = new Stopwatch();

            FormulaManager manager = null;

            while (true)
            {
                try
                {
                    stopWatch.Reset();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Enter formula or press CTRL-C to close:");
                    var line = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    manager = new FormulaManager(line);
                    double result = 0;

                    var eval = manager.CreateFormulaEvaluator();

                    eval.OptimiseFormula(_optimisationLevel);

                    var varMap = new Dictionary<string, double>();

                    if (eval.RequiresVariables)
                    {
                        Console.WriteLine("Fill Variables:");
                        foreach (var variable in eval.RequiredVariables)
                        {
                            while (true)
                            {
                                Console.Write($"{variable}: ");
                                var varValue = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(varValue))
                                {
                                    double v;
                                    if (double.TryParse(varValue, out v))
                                    {
                                        varMap.Add(variable, v);
                                        break;
                                    }
                                }
                            }

                        }
                    }
                    Console.WriteLine();
                    WritePair("Input Formula: ", eval.OriginalFormula);
                    WritePair("Parsed Formula: ", eval.ParsedFormula);
                    WritePair("Raw Parsed Formula: ", eval.RawParsedFormula);

                    if (_optimisationLevel == FormulaOptimisationLevel.Compiled)
                    {
                        PokeAroundForDebugInfo(eval);
                    }

                    eval.SetVariableMap(varMap);

                    stopWatch.Start();
                    result = eval.GetResult();
                    stopWatch.Stop();


                    WritePair("Result: ", result.ToString());
                    var rresult = Math.Round(result, 8);
                    WritePair("Rounded Result: ", rresult.ToString());
                    WritePair("Time to execute: ", $" ~{stopWatch.ElapsedMilliseconds}");
                    Console.WriteLine();
                }
                catch (FormulaException ex)
                {
                    var longMsg = ex.TryGetFailurePointMessage(manager.InputFormula);
                    var msg = string.IsNullOrWhiteSpace(longMsg) ? ex.Message : longMsg;
                    using (ConsoleForeAndBackColourHelper.FromColourPair(ConsoleColourPairs.RedWarning))
                    {
                        Console.WriteLine();
                        Console.WriteLine("Error while executing formula:");
                        Console.WriteLine(msg);
                    }
                }
            }
        }

        private static void PokeAroundForDebugInfo(IFormulaEvaluator eval)
        {
            // Do naughty things here and poke around the class's guts:
            var m = eval.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var info in m)
            {
                Console.WriteLine(info.Name);
            }
        }

        private static void WritePair(string s1, string s2)
        {
            Console.Write(s1);
            using (ConsoleForeAndBackColourHelper.FromColourPair(ConsoleColourPairs.BlueHighlight))
            {
                Console.Write(s2);
            }
            Console.WriteLine();
        }
    }
}
