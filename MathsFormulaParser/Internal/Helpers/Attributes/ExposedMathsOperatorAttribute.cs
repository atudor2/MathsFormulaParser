using System;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Operators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes
{
    /// <summary>
    /// Attribute used to expose a method as a operator
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class ExposedMathsOperatorAttribute : AbstractExposedMathsAttribute
    {
        /// <summary>
        /// Symbol for the operator. CANNOT BE NULL
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
    }
}
