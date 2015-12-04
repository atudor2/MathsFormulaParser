using Alistair.Tudor.MathsFormulaParser.Internal.Functions;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    /// <summary>
    /// Helper extension methods for Functions.
    /// Mainly for the Parser where functions are treated like 'operators'
    /// </summary>
    internal static class FunctionExtensions
    {
        private const OperatorAssociativity DefaultFunctionAssociativity = OperatorAssociativity.Left;
        private const int DefaultFunctionPrecedence = OperatorConstants.FunctionPrecedence;

        /// <summary>
        /// If the given function is an operator, returns the Associativity otherwise returns the default for normal functions
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public static OperatorAssociativity GetAssociativity(this StandardFunction function)
        {
            return (function as Operator)?.Associativity ?? DefaultFunctionAssociativity;
        }

        /// <summary>
        /// /// If the given function is an operator, returns the Precedence otherwise returns the default for normal functions
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public static int GetPrecedence(this StandardFunction function)
        {
            return (function as Operator)?.Precedence ?? DefaultFunctionPrecedence;
        }
    }
}
