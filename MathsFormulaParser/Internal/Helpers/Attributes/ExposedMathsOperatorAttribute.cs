using System;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes
{
    /// <summary>
    /// Attribute used to expose a method as a operator
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class ExposedMathsOperatorAttribute : Attribute
    {
        /// <summary>
        /// Symbol for the operator. If null, the method name is used
        /// </summary>
        public string OperatorSymbol { get; set; } = null;

        /// <summary>
        /// Operator Precedence
        /// </summary>
        public int Precedence { get; set; } = 0;

        /// <summary>
        /// Operator Associativity. Defaults to LEFT
        /// </summary>
        public OperatorAssociativity Associativity { get; set; } = OperatorAssociativity.Left;

        /// <summary>
        /// Gets or sets the number of arguments required for the function
        /// </summary>
        public int RequiredArgumentCount { get; set; }
    }
}
