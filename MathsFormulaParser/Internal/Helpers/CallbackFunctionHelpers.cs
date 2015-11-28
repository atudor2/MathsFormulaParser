using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    internal static class CallbackFunctionHelpers
    {
        public static bool IsValidFormulaCallbackFunctionMethod(MethodInfo method)
        {
            return method.IsMethodCompatibleWithDelegate<FormulaCallbackFunction>();
        }

        public static FormulaCallbackFunction CreateCallbackFunctionDelegate(MethodInfo method)
        {
            return (FormulaCallbackFunction)Delegate.CreateDelegate(typeof(FormulaCallbackFunction), method);
        }
    }
}
