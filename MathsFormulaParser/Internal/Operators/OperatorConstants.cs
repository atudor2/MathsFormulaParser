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

        /// <summary>
        /// Default callback for dividing operations that require checking if the divisor is zero
        /// </summary>
        /// <param name="input"></param>
        public static void DividingOperatorExtendedCheck(double[] input)
        {
            var divisor = input.Last();
            if (divisor == 0)
            {
                // Cannot divide by 0!
                throw new OperatorExtendedCheckException("Cannot divide by zero");
            }
        }
    }
}
