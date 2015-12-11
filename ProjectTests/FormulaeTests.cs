using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Alistair.Tudor.MathsFormulaParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectTests
{
    [TestClass]
    public class FormulaeTests
    {
        private readonly List<Tuple<string, double>> _goodFormulae = new List<Tuple<string, double>>()
        {
           new Tuple<string, double>("5 ^ 3", 6),
           new Tuple<string, double>("5 + 3", 8),
          new Tuple<string, double>("5[1]", 1),
          new Tuple<string, double>("((100*5)/100)[5-4] * A", 6),
            //new Tuple<string, double>("5[1] + 5", 6),
            //new Tuple<string, double>("Max(5[1], 5)", 5),
            new Tuple<string, double>("Max(Rad2Deg(PI), 5)", 180),
            //new Tuple<string, double>("pow(sqrt(2 * log(100)), 3)", 8),
            new Tuple<string, double>("3 + 4 + sin(11)", 6.00000979),
            new Tuple<string, double>("(((A +B)-C)*9**2)", 324),
           new Tuple<string, double>("-2", -2),
           new Tuple<string, double>("((A + B - (33 + 3)) * 2)", -56),
        };

        private readonly Dictionary<string, double> _varMap = new Dictionary<string, double>()
        {
            {"A", 6.0},
            {"B", 2.0},
            {"C", 4.0}
        };

        //[TestMethod]
        //public void Test_Memory()
        //{
        //    var rnd = new Random();
        //    while (true)
        //    {
        //        var f = _goodFormulae[rnd.Next(0, _goodFormulae.Count)];

        //        TimeExpression(f, 250, 3);
        //    }
        //}

        [TestMethod]
        public void Test_Huge_Inline_Function_Calls()
        {
            var manager = new FormulaManager("5AD");//"pow(sqrt(2 * log(100, 10)), 3)");
            var evaluator = manager.CreateFormulaEvaluator();
            evaluator.OptimiseFormula();
            var r = evaluator.GetResult();
            Debug.Assert(((int)r) == -200, "r != -200");
        }

        [TestMethod]
        public void Test_Perf_Not_Bad_Random()
        {
            var rnd = new Random();
            var f = _goodFormulae[rnd.Next(0, _goodFormulae.Count)];
            TimeExpression(f);
        }

        [TestMethod]
        public void Test_Perf_Not_Bad()
        {
            foreach (var tuple in _goodFormulae)
            {
                TimeExpression(tuple);
                Console.WriteLine();
            }
        }

        private void TimeExpression(Tuple<string, double> f, double max = 1000000, double threshold = 0.06)
        {
            var manager = new FormulaManager(f.Item1);

            var evaluator = manager.CreateFormulaEvaluator();
            evaluator.OptimiseFormula(FormulaOptimisationLevel.Basic);
            evaluator.SetVariableMap(_varMap);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (var i = 0; i < max; i++)
            {
                evaluator.GetResult();
            }

            stopwatch.Stop();

            var tpc = stopwatch.ElapsedMilliseconds / max;

            if (tpc >= threshold)
            {
                // FAIL:
                Debug.Fail($"Average execution for {max} iterations was not <= 0.06");
            }
            Console.WriteLine($"Executed {max} evaluations of '{f.Item1}' at ~{tpc}ms per evaluation");
        }

        [TestMethod]
        public void Test_Valid_Formulae_And_Results_No_Optimisation()
        {
            foreach (var tuple in _goodFormulae)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Input Formula: {tuple.Item1}");

                var manager = new FormulaManager(tuple.Item1);
                var evaluator = manager.CreateFormulaEvaluator();
                evaluator.SetVariableMap(_varMap);

                Console.WriteLine($"Parsed Formula: {evaluator.ParsedFormula}");
                Console.WriteLine($"Raw Parsed Formula: {evaluator.RawParsedFormula}");

                var result = evaluator.GetResult();

                Console.WriteLine($"Result: {result}");

                var rresult = Math.Round(result, 8);

                Console.WriteLine($"Rounded Result: { rresult }");

                Console.WriteLine($"Expected Result: { tuple.Item2 }");

                Debug.Assert(Math.Abs(rresult - tuple.Item2) < 0.0000001, $"'{rresult}' != '{tuple.Item2}'");
            }

        }
    }
}
