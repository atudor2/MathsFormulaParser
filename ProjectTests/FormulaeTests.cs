using System;
using System.Collections.Generic;
using System.Diagnostics;
using Alistair.Tudor.MathsFormulaParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectTests
{
    [TestClass]
    public class FormulaeTests
    {
        [TestMethod]
        public void TestValidFormulaeAndResults()
        {
            var manager = new FormulaManager("((A +B)-C)*9**2");
            var evaluator = manager.CreateFormulaEvaluator();
            evaluator.PerformExtendedChecks = true;
            evaluator.OptimiseFormula();
            evaluator.SetVariableMap(new Dictionary<string, double>()
            {
                { "A", 6.0 },
                { "B", 2.0 },
                { "C", 4.0 }
            });

            double max = 10000;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < max; i++)
            {
                evaluator.GetResult(); // 324
            }

            stopwatch.Stop();

            var tpc = stopwatch.ElapsedMilliseconds/max;

            var r = evaluator.GetResult(); // 324
            Debug.Assert((int)r == 12, "r == 12");
        }
    }
}
