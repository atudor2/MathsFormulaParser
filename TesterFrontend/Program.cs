using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alistair.Tudor.ConsoleUtil;
using Alistair.Tudor.MathsFormulaParser;

namespace TesterFrontend
{
    // NB: Dirty code ahead - this is not my usual quality!
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
                var items = Enum.GetNames(typeof (FormulaOptimisationLevel));
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
                        (FormulaOptimisationLevel) Enum.Parse(typeof (FormulaOptimisationLevel), items[int.Parse(s) - 1]);
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

            while (true)
            {
                stopWatch.Reset();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Enter formula or press CTRL-C to close:");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var manager = new FormulaManager(line);
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

                Console.WriteLine($"Input Formula: {eval.OriginalFormula}");
                Console.WriteLine($"Parsed Formula: {eval.ParsedFormula}");
                Console.WriteLine($"Raw Parsed Formula: {eval.RawParsedFormula}");

                eval.SetVariableMap(varMap);

                stopWatch.Start();
                var result = eval.GetResult();
                stopWatch.Stop();

                Console.WriteLine($"Result: {result}");
                var rresult = Math.Round(result, 8);
                Console.WriteLine($"Rounded Result: {rresult}");
                Console.WriteLine($"Time to execute ~{stopWatch.ElapsedMilliseconds}");
                Console.WriteLine();
            }
        }
    }
}
