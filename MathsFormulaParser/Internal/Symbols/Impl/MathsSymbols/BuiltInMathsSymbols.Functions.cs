using System;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes;

// ReSharper disable once CheckNamespace
namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl
{
    /// <summary>
    /// Internal class containing implementations of the default maths operators and functions
    /// </summary>
    // This portion contains the functions
    internal static partial class BuiltInMathsSymbols
    {
        [ExposedMathFunction(RequiredArgumentCount = 2, FunctionName = "ensure_l_bound")]
        public static double EnsureLowerBound(double[] inputArray)
        {
            var input = inputArray[0];
            var min = inputArray[1];

            if (input < min)
            {
                return min;
            }
            return input;
        }

        [ExposedMathFunction(RequiredArgumentCount = 2, FunctionName = "ensure_u_bound")]
        public static double EnsureUpperBound(double[] inputArray)
        {
            var input = inputArray[0];
            var max = inputArray[1];

            if (input > max)
            {
                return max;
            }
            return input;
        }

        [ExposedMathFunction(RequiredArgumentCount = 3)]
        public static double EnsureBounds(double[] inputArray)
        {
            var input = inputArray[0];
            var min = inputArray[1];
            var max = inputArray[2];

            // Series of 'transformations'
            input = EnsureLowerBound(new[] { input, min });
            input = EnsureUpperBound(new[] { input, max });
            return input;
        }

        [ExposedMathFunction(RequiredArgumentCount = 1)]
        public static double Deg2Rad(double[] input)
        {
            var degree = input[0];
            return degree * (Math.PI / 180);
        }

        [ExposedMathFunction(RequiredArgumentCount = 3)]
        public static double If(double[] input)
        {
            var condition = input[0];
            var trueResult = input[1];
            var falseResult = input[2];
            return Double2Bool(condition) ? trueResult : falseResult;
        }

        [ExposedMathFunction(RequiredArgumentCount = 2)]
        public static double GetBit(double[] input)
        {
            var number = (int)input[0];
            var bitPosition = (int)input[1];
            if (bitPosition <= 0) throw new ArgumentOutOfRangeException(nameof(bitPosition), "Bit position cannot be less than 1");
            bitPosition--; // 0-Based - 

            // E.g.:
            // 11: 1 0 1 1
            // Get bit at position 2:
            //   1011
            // & 0010 
            // =======
            //   0010 => 2
            // Versus @ 3:
            //   1011
            // & 0100 
            // =======
            //   0000 => 0 => Bit not set therefore 0
            return (number & (1 << bitPosition)) == 0 ? 0 : 1;
        }

        [ExposedMathFunction(RequiredArgumentCount = 1)]
        public static double Rad2Deg(double[] input)
        {
            var radians = input[0];
            return radians * (180 / Math.PI);
        }

        [ExposedMathFunction(RequiredArgumentCount = 1)]
        public static double ToBool(double[] input)
        {
            return Bool2Int((int)input[0] != 0);
        }
    }
}
