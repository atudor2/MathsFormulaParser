using Alistair.Tudor.MathsFormulaParser.Internal.Functions;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    internal static class FunctionExtensions
    {
        private const OperatorAssociativity DefaultFunctionAssociativity = OperatorAssociativity.Left;
        private const int DefaultFunctionPrecedence = OperatorConstants.FunctionPrecedence;

        public static OperatorAssociativity GetAssociativity(this StandardFunction function)
        {
            return (function as Operator)?.Associativity ?? DefaultFunctionAssociativity;
        }

        public static int GetPrecedence(this StandardFunction function)
        {
            return (function as Operator)?.Precedence ?? DefaultFunctionPrecedence;
        }
    }
}
