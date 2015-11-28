using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Operators.Impl
{
    /// <summary>
    /// Internal class containing implementations of maths operators
    /// </summary>
    internal static class MathsOperators
    {
        [ExposedMathsOp(OperatorSymbol = "+", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static double AddOp(double x, double y)
        {
            return x + y;
        }

        [ExposedMathsOp(OperatorSymbol = "&", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static int AndOp(int x, int y)
        {
            return x & y;
        }

        [ExposedMathsOp(OperatorSymbol = "<<", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static double BitLeftOp(double x, double y)
        {
            return (int)x << (int)y;
        }

        [ExposedMathsOp(OperatorSymbol = ">>", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static int BitRightOp(int x, int y)
        {
            return x >> y;
        }

        [ExposedMathsOp(Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left)]
        public static double Deg2Rad(double degree)
        {
            return degree * (Math.PI / 180);
        }

        [InternalMarkerAttr(OpType = InternalMarkerAttr.DivType)]
        [ExposedMathsOp(OperatorSymbol = "/", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static double DivOp(double x, double y)
        {
            return x / y;
        }

        [ExposedMathsOp(Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left)]
        public static int GetBit(int number, int bitPosition)
        {
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
        [ExposedMathsOp(OperatorSymbol = "%", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static double ModOp(double x, double y)
        {
            return x % y;
        }

        [ExposedMathsOp(OperatorSymbol = "~", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Right)]
        public static int NotOp(int x)
        {
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

        [ExposedMathsOp(OperatorSymbol = "|", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static int OrOp(int x, int y)
        {
            return x | y;
        }

        [ExposedMathsOp(OperatorSymbol = "**", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Right)]
        public static double PowOp(double x, double y)
        {
            return Math.Pow(x, y);
        }

        [ExposedMathsOp(OperatorSymbol = "*", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static double ProductOp(double x, double y)
        {
            return x * y;
        }

        [ExposedMathsOp(Precedence = OperatorConstants.FunctionPrecedence, Associativity = OperatorAssociativity.Left)]
        public static double Rad2Deg(double radians)
        {
            return radians * (180 / Math.PI);
        }

        [ExposedMathsOp(OperatorSymbol = "-", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static double SubOp(double x, double y)
        {
            return x - y;
        }

        [ExposedMathsOp(OperatorSymbol = "^", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left)]
        public static int XOrOp(int x, int y)
        {
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

                FormulaCallbackFunction func;
                Operator op;

                // Assign the thunk:
                if (method.ReturnType == typeof(int))
                {
                    func = (i) => OperatorThunk(method, i.Select(x => (int)x));
                }
                else
                {
                    //func = (FormulaCallbackFunction) Delegate.CreateDelegate(typeof (FormulaCallbackFunction), method);
                    func = (i) => OperatorThunk(method, i);
                }

                if (markerAttr != null && markerAttr.OpType == InternalMarkerAttr.DivType)
                {
                    // Divisor 
                    op = new DividingOperator(attr.Precedence, operatorValue, attr.Associativity, @params.Length,
                        func);

                }
                else
                {
                    op = new GenericOperator(attr.Precedence, operatorValue, attr.Associativity, @params.Length,
                        func);
                }
                yield return op;
            }
        }

        private static bool IsValidMethod(MethodInfo method)
        {
            var @params = method.GetParameters();

            var validDoubleMethod = @params.Any(p => p.ParameterType != typeof (double)) &&
                                    method.ReturnType != typeof (double);

            var validIntMethod = @params.Any(p => p.ParameterType != typeof(int)) &&
                                    method.ReturnType != typeof(int);

            return validDoubleMethod || validIntMethod;
        }
        [AttributeUsage(AttributeTargets.Method)]
        private class InternalMarkerAttr : Attribute
        {
            public const int DivType = 1;
            public int OpType { get; set; } = 0;
        }
    }
}
