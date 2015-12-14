using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Operators;

// ReSharper disable once CheckNamespace
namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl
{
    /// <summary>
    /// Internal class containing implementations of the default maths operators and functions
    /// </summary>
    // This portion contains unary operators
    internal static partial class BuiltInMathsSymbols
    {
        [ExposedMathsOperator(OperatorSymbol = "-", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 1)]
        public static double UnaryNegative(double[] input)
        {
            return -input[0];
        }

        [ExposedMathsOperator(OperatorSymbol = "~", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 1)]
        public static double Not(double[] input)
        {
            var x = (int)input[0];
            return ~x;
        }

        [ExposedMathsOperator(OperatorSymbol = "!", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 1)]
        public static double LogicalNot(double[] input)
        {
            return Bool2Int(BooleanOp(input, "!"));
        }
    }
}
