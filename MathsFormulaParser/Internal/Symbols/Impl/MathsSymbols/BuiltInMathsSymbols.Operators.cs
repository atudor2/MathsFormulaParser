using System;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Operators;

// ReSharper disable once CheckNamespace
namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl
{
    /// <summary>
    /// Internal class containing implementations of the default maths operators and functions
    /// </summary>
    // This portion contains the operators
    internal static partial class BuiltInMathsSymbols
    {
        [ExposedMathsOperator(OperatorSymbol = "!@", Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double GetBitOperator(double[] input)
        {
            // Forward the call:
            return GetBit(input);
        }

        [ExposedMathsOperator(OperatorSymbol = "+", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Add(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x + y;
        }

        [ExposedMathsOperator(OperatorSymbol = "&", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double And(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x & y;
        }

        [ExposedMathsOperator(OperatorSymbol = "<<", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double BitLeft(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return (int)x << (int)y;
        }

        [ExposedMathsOperator(OperatorSymbol = ">>", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double BitRight(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x >> y;
        }

        [ExposedMathsOperator(OperatorSymbol = "/", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Divide(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x / y;
        }

        [ExposedMathsOperator(OperatorSymbol = "%", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Mod(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x % y;
        }

        [ExposedMathsOperator(OperatorSymbol = "|", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Or(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x | y;
        }

        [ExposedMathsOperator(OperatorSymbol = "**", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 2)]
        public static double Power(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return Math.Pow(x, y);
        }

        [ExposedMathsOperator(OperatorSymbol = "*", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Product(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x * y;
        }

        [ExposedMathsOperator(OperatorSymbol = "-", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Subtract(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x - y;
        }

        [ExposedMathsOperator(OperatorSymbol = "^", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double XOr(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x ^ y;
        }
    }
}
