using System.Collections.Generic;
using System.Reflection;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Functions.Impl
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
