using System;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes
{
    /// <summary>
    /// Attribute used to expose a method as a function
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class ExposedMathFunctionAttribute : AbstractExposedMathsAttribute
    {
        /// <summary>
        /// Function name. If null, the method name is used
        /// </summary>
        public string FunctionName { get; set; } = null;
    }
}
