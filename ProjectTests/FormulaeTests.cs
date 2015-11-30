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

        [TestMethod]
        public void x111()
        {

            var powInfo = typeof(Math).GetMethod("Pow");

            double max = 10000;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < max; i++)
            {
                var xxx = powInfo.Invoke(null, new object[] { 2, 5 }); // 2^5 == 32
            }
            stopwatch.Stop();
            var tpc1 = stopwatch.ElapsedMilliseconds / max;

            stopwatch.Reset();

            var paramTypes = powInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            var returnType = powInfo.ReturnType;

            var typeUnion = paramTypes.Concat(new[] { returnType }).ToArray();

            Type genericFuncType;
            MethodCallExpression methodCallExpr;

            FormulaCallbackFunction callbackFunctionThunk = null;

            // NB: Need to specify the arity oif the FUNC
            switch (paramTypes.Length)
            {
                case 0:
                    {
                        methodCallExpr = Expression.Call(powInfo, paramTypes.Select(Expression.Parameter));
                        dynamic d = Expression.Lambda(methodCallExpr).Compile();
                        callbackFunctionThunk = (input) => d();
                    }
                    break;
                case 1:
                    genericFuncType = typeof(Func<,>);
                    break;
                case 2:
                    {
                        var @params = paramTypes.Select(Expression.Parameter).ToArray();
                        methodCallExpr = Expression.Call(powInfo, @params);
                        dynamic d = Expression.Lambda<Func<double, double, double>>(methodCallExpr, @params).Compile();
                        callbackFunctionThunk = (input) => d(input[0], input[1]);
                    }
                    break;
                case 3:
                    genericFuncType = typeof(Func<,,,>);
                    break;
                default:
                    // Fallback to a MethodInfo.Invoke() call:
                    genericFuncType = null;
                    break;
            }

            //var delegateFuncType = genericFuncType.MakeGenericType(typeUnion);

            //var @delegate = Delegate.CreateDelegate(delegateFuncType, powInfo);



            //Func expressionLambda =


            stopwatch.Start();
            for (int i = 0; i < max; i++)
            {
                var xxx = callbackFunctionThunk(new[] { 2D, 5D }); // 2^5 == 32
            }
            stopwatch.Stop();
            var tpc2 = stopwatch.ElapsedMilliseconds / max;

        }

        [TestMethod]
        public void TestValidFormulaeAndResults()
        {
            var manager = new FormulaManager("(((A +B)-C)*9**2) + Pow(((2 * PI) - PI), 45)");
            var evaluator = manager.CreateFormulaEvaluator();
            evaluator.PerformExtendedChecks = true;
            evaluator.OptimiseFormula();
            evaluator.SetVariableMap(new Dictionary<string, double>()
            {
                { "A", 6.0 },
                { "B", 2.0 },
                { "C", 4.0 }
            });

            double max = 1000000;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < max; i++)
            {
                evaluator.GetResult(); // 324
            }

            stopwatch.Stop();

            var tpc = stopwatch.ElapsedMilliseconds / max;

            var r = evaluator.GetResult(); // 324
            var xx = evaluator.ParsedFormula;
            var zz = evaluator.RawParsedFormula;
            Debug.Assert((int)r == 324, "r == 324");
        }
    }
}
