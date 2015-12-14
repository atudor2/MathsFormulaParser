using System;
using System.Collections.Generic;
using System.Reflection;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Functions;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Operators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl
{
    /// <summary>
    /// Internal class containing implementations of the default maths operators and functions
    /// </summary>
    // This portion contains the general logic
    internal static partial class BuiltInMathsSymbols
    {
        /// <summary>
        /// Gets a list of operators and functions within this class
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<FormulaFunction> GetOperatorsAndFunctions()
        {
            var mathType = typeof(BuiltInMathsSymbols);
            var methods = mathType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<AbstractExposedMathsAttribute>();
                if (attr == null) continue; // Skip...

                if (!IsValidMethod(method))
                {
                    continue;
                }

                // Create the delegate:
                var func = CallbackFunctionHelpers.CreateCallbackFunctionDelegate(method);

                var methodName = method.Name.ToLower();

                // Make the object:
                if (attr is ExposedMathsOperatorAttribute)
                {
                    var op = (ExposedMathsOperatorAttribute)attr;
                    yield return new Operator(op.OperatorSymbol, op.Precedence, op.Associativity, func, op.RequiredArgumentCount, methodName);
                }
                else
                {
                    var f = (ExposedMathFunctionAttribute)attr;
                    var funcName = string.IsNullOrWhiteSpace(f.FunctionName) ? methodName : f.FunctionName;
                    yield return new StandardFunction(funcName, func, f.RequiredArgumentCount);
                }
            }
        }

        /// <summary>
        /// Converts a boolean to an integer
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static int Bool2Int(bool x)
        {
            return x ? 1 : 0;
        }

        /// <summary>
        /// Converts an integer to a boolean
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool Double2Bool(double x)
        {
            return x != 0;
        }

        /// <summary>
        /// Handles boolean operations
        /// </summary>
        /// <param name="input"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        private static bool BooleanOp(double[] input, string op)
        {
            switch (op)
            {
                case "||":
                    return Double2Bool(input[0]) || Double2Bool(input[1]);
                case "&&":
                    return Double2Bool(input[0]) && Double2Bool(input[1]);
                case "==":
                    {
                        var d1 = input[0];
                        var d2 = input[1];
                        var difference = Math.Abs(d1 * .00001);
                        return Math.Abs(d1 - d2) < difference;
                    }
                case "!":
                    return !Double2Bool(input[0]);
                case ">":
                    return input[0] > input[1];
                case ">=":
                    return input[0] >= input[1];
                case "<":
                    return input[0] < input[1];
                case "<=":
                    return input[0] <= input[1];
                case "!=":
                    return !BooleanOp(input, "==");
                default:
                    throw new InvalidOperationException($"Invalid operator {op}");
            }
        }

        /// <summary>
        /// Gets whether the given MethodInfo is a valid FormulaCallbackFunction 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static bool IsValidMethod(MethodInfo method)
        {
            return CallbackFunctionHelpers.IsValidFormulaCallbackFunctionMethod(method);
        }
    }
}
