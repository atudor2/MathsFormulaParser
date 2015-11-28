using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Operators.Impl
{
    /// <summary>
    /// Internal class containing implementations of maths operators
    /// </summary>
    internal static class MathsOperators
    {
        [ExposedMathsOp(OperatorSymbol = "+", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount =  2)]
        public static double AddOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x + y;
        }

        [ExposedMathsOp(OperatorSymbol = "&", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double AndOp(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x & y;
        }

        [ExposedMathsOp(OperatorSymbol = "<<", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double BitLeftOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return (int)x << (int)y;
        }

        [ExposedMathsOp(OperatorSymbol = ">>", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static int BitRightOpAndOp(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x >> y;
        }

        [ExposedMathsOp(Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 1)]
        public static double Deg2Rad(double[] input)
        {
            var degree = input[0];
            return degree * (Math.PI / 180);
        }

        [InternalMarkerAttr(OpType = InternalMarkerAttr.DivType)]
        [ExposedMathsOp(OperatorSymbol = "/", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double DivOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x / y;
        }

        [ExposedMathsOp(Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static int GetBit(double[] input)
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
        [ExposedMathsOp(OperatorSymbol = "%", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double ModOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x % y;
        }

        [ExposedMathsOp(OperatorSymbol = "~", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 2)]
        public static double NotOp(double[] input)
        {
            var x = (int)input[0];
            return ~x;
        }

        /// <summary>
        /// Thunk to call into a given operator method
        /// </summary>
        /// <param name="method"></param>
        /// <param name="rawInput"></param>
        /// <returns></returns>
        public static T OperatorThunk<T>(MethodInfo method, IEnumerable<T> rawInput) where T : struct
        {
            var input = rawInput.Select(x => (object)x).ToArray();
            return (T)method.Invoke(null, input);
        }

        [ExposedMathsOp(OperatorSymbol = "|", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double OrOp(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x | y;
        }

        [ExposedMathsOp(OperatorSymbol = "**", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 2)]
        public static double PowOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return Math.Pow(x, y);
        }

        [ExposedMathsOp(OperatorSymbol = "*", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double ProductOp(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x * y;
        }

        [ExposedMathsOp(Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 1)]
        public static double Rad2Deg(double[] input)
        {
            var radians = input[0];
            return radians * (180 / Math.PI);
        }

        [ExposedMathsOp(OperatorSymbol = "-", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double SubO(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x - y;
        }

        [ExposedMathsOp(OperatorSymbol = "^", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
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
                var attr = method.GetCustomAttribute<ExposedMathsOpAttribute>();
                if (attr == null) continue; // Skip...

                var markerAttr = method.GetCustomAttribute<InternalMarkerAttr>();

                if (!IsValidMethod(method))
                {
                    continue;
                }

                var @params = method.GetParameters();

                var operatorValue = string.IsNullOrWhiteSpace(attr.OperatorSymbol)
                    ? method.Name.ToLower()
                    : attr.OperatorSymbol;

                Operator op;

                // Create te delegate:
                var func = CallbackFunctionHelpers.CreateCallbackFunctionDelegate(method);

                if (markerAttr != null && markerAttr.OpType == InternalMarkerAttr.DivType)
                {
                    // Divisor 
                    op = new DividingOperator(attr.Precedence, operatorValue, attr.Associativity, attr.RequiredArgumentCount,
                        func);

                }
                else
                {
                    op = new GenericOperator(attr.Precedence, operatorValue, attr.Associativity, attr.RequiredArgumentCount,
                        func);
                }
                yield return op;
            }
        }

        private static bool IsValidMethod(MethodInfo method)
        {
            return CallbackFunctionHelpers.IsValidFormulaCallbackFunctionMethod(method);
        }
        [AttributeUsage(AttributeTargets.Method)]
        private class InternalMarkerAttr : Attribute
        {
            public const int DivType = 1;
            public int OpType { get; set; } = 0;
        }
    }
}
