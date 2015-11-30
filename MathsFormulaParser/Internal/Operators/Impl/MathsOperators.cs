using System;
using System.Collections.Generic;
using System.Reflection;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Operators.Impl
{
    /// <summary>
    /// Internal class containing implementations of the default maths operators
    /// </summary>
    internal static class MathsOperators
    {
        [ExposedMathsOperator(OperatorSymbol = "+", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount =  2, IsSymbolicOperator = true)]
        public static double AddOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x + y;
        }

        [ExposedMathsOperator(OperatorSymbol = "&", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double AndOp(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x & y;
        }

        [ExposedMathsOperator(OperatorSymbol = "<<", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double BitLeftOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return (int)x << (int)y;
        }

        [ExposedMathsOperator(OperatorSymbol = ">>", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static int BitRightOp(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x >> y;
        }

        [ExposedMathsOperator(Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 1)]
        public static double Deg2Rad(double[] input)
        {
            var degree = input[0];
            return degree * (Math.PI / 180);
        }

        [InternalMarkerAttr(OpType = InternalMarkerAttr.DivType)]
        [ExposedMathsOperator(OperatorSymbol = "/", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double DivOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x / y;
        }

        [ExposedMathsOperator(Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
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

        [InternalMarkerAttr(OpType = InternalMarkerAttr.DivType)]
        [ExposedMathsOperator(OperatorSymbol = "%", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double ModOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x % y;
        }

        [ExposedMathsOperator(OperatorSymbol = "~", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double NotOp(double[] input)
        {
            var x = (int)input[0];
            return ~x;
        }

        [ExposedMathsOperator(OperatorSymbol = "|", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double OrOp(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x | y;
        }

        [ExposedMathsOperator(OperatorSymbol = "**", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double PowOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return Math.Pow(x, y);
        }

        [ExposedMathsOperator(OperatorSymbol = "*", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double ProductOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x * y;
        }

        [ExposedMathsOperator(Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 1)]
        public static double Rad2Deg(double[] input)
        {
            var radians = input[0];
            return radians * (180 / Math.PI);
        }

        [ExposedMathsOperator(OperatorSymbol = "-", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double SubOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x - y;
        }

        [ExposedMathsOperator(OperatorSymbol = "^", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2, IsSymbolicOperator = true)]
        public static double XOrOp(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x ^ y;
        }

        /// <summary>
        /// Gets a list of operators within this class
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Operator> GetOperators()
        {
            var mathType = typeof(MathsOperators);
            var methods = mathType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ExposedMathsOperatorAttribute>();
                if (attr == null) continue; // Skip...

                var markerAttr = method.GetCustomAttribute<InternalMarkerAttr>();

                if (!IsValidMethod(method))
                {
                    continue;
                }

                var operatorValue = string.IsNullOrWhiteSpace(attr.OperatorSymbol)
                    ? method.Name.ToLower()
                    : attr.OperatorSymbol;

                Operator op;

                // Create the delegate:
                var func = CallbackFunctionHelpers.CreateCallbackFunctionDelegate(method);

                if (markerAttr != null && markerAttr.OpType == InternalMarkerAttr.DivType)
                {
                    // Divisor 
                    op = new DividingOperator(attr.Precedence, operatorValue, attr.Associativity, attr.RequiredArgumentCount,
                        func, attr.IsSymbolicOperator);

                }
                else
                {
                    op = new GenericOperator(attr.Precedence, operatorValue, attr.Associativity, attr.RequiredArgumentCount,
                        func, attr.IsSymbolicOperator);
                }
                yield return op;
            }
        }

        /// <summary>
        /// Gets whether the given MethodInfo is a valid FormulaCallbackFunction 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static bool IsValidMethod(MethodInfo method)
        {
            return CallbackFunctionHelpers.IsValidFormulaCallbackFunctionMethod(method);
        }

        /// <summary>
        /// Internal helper marker attribute for dividing operators
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        private class InternalMarkerAttr : Attribute
        {
            public const int DivType = 1;
            public int OpType { get; set; } = 0;
        }
    }
}
