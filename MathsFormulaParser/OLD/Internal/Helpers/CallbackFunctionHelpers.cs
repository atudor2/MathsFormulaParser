using System;
using System.Reflection;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    /// <summary>
    /// Helper class for working with the FormulaCallbackFunction delegate
    /// </summary>
    internal static class CallbackFunctionHelpers
    {
        /// <summary>
        /// Checks if the given MethodInfo is compatible with FormulaCallbackFunction
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsValidFormulaCallbackFunctionMethod(MethodInfo method)
        {
            return method.IsMethodCompatibleWithDelegate<FormulaCallbackFunction>();
        }

        /// <summary>
        /// Creates a delegate for FormulaCallbackFunction from a MethodInfo
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static FormulaCallbackFunction CreateCallbackFunctionDelegate(MethodInfo method)
        {
            return (FormulaCallbackFunction)Delegate.CreateDelegate(typeof(FormulaCallbackFunction), method);
        }
    }
}
