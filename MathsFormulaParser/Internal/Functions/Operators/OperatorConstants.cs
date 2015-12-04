using System.Linq;
using Alistair.Tudor.MathsFormulaParser.Internal.Exceptions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Operators
{
    /// <summary>
    /// Special constants for constructing operators
    /// </summary>
    internal static class OperatorConstants
    {
        /// <summary>
        /// Precedence value for functions
        /// </summary>
        public const int FunctionPrecedence = 17;

        /// <summary>
        /// Precedence value for division or multiplication
        /// </summary>
        public const int DivMultOpsPrecedence = 14;

        /// <summary>
        /// Precedence value for addition or subtraction
        /// </summary>
        public const int AddSubOpsPrecedence = 13;

        /// <summary>
        /// Precedence value for bit wise operations
        /// </summary>
        public const int BitOpsPrecedence = 12;
    }
}
