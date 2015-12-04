using System;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes
{
    /// <summary>
    /// Base class for exposed maths items
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class AbstractExposedMathsAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the number of arguments required for the function
        /// </summary>
        public int RequiredArgumentCount { get; set; }
    }
}
