﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Attributes;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Functions.Impl
{
    /// <summary>
    /// Internal class containing implementations of the default maths operators
    /// </summary>
    internal static class MathsOperators
    {
        [ExposedMathsOperator(OperatorSymbol = "+", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount =  2)]
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
        public static int BitRight(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x >> y;
        }

        [ExposedMathFunction(RequiredArgumentCount = 1)]
        public static double Deg2Rad(double[] input)
        {
            var degree = input[0];
            return degree * (Math.PI / 180);
        }

        [ExposedMathsOperator(OperatorSymbol = "/", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Div(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x / y;
        }

        [ExposedMathFunction(RequiredArgumentCount = 1)]
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

        [ExposedMathsOperator(OperatorSymbol = "%", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Mod(double[] input)
        {
            var x = input[0];
            var y = input[1];
            return x % y;
        }

        [ExposedMathsOperator(OperatorSymbol = "~", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 2)]
        public static double Not(double[] input)
        {
            var x = (int)input[0];
            return ~x;
        }

        [ExposedMathsOperator(OperatorSymbol = "|", Precedence = OperatorConstants.BitOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Or(double[] input)
        {
            var x = (int)input[0];
            var y = (int)input[1];
            return x | y;
        }

        [ExposedMathsOperator(OperatorSymbol = "**", Precedence = OperatorConstants.DivMultOpsPrecedence, Associativity = OperatorAssociativity.Right, RequiredArgumentCount = 2)]
        public static double Pow(double[] input)
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

        [ExposedMathFunction(RequiredArgumentCount = 1)]
        public static double Rad2Deg(double[] input)
        {
            var radians = input[0];
            return radians * (180 / Math.PI);
        }

        [ExposedMathsOperator(OperatorSymbol = "-", Precedence = OperatorConstants.AddSubOpsPrecedence, Associativity = OperatorAssociativity.Left, RequiredArgumentCount = 2)]
        public static double Sub(double[] input)
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

        /// <summary>
        /// Gets a list of operators within this class
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<StandardFunction> GetOperators()
        {
            var mathType = typeof(MathsOperators);
            var methods = mathType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<AbstractExposedMathsAttribute>();
                if (attr == null) continue; // Skip...

                if (!IsValidMethod(method))
                {
                    continue;
                }

                // Create the delegate:
                var func = CallbackFunctionHelpers.CreateCallbackFunctionDelegate(method);

                var methodName = method.Name.ToLower();

                // Make the object:
                if (attr is ExposedMathsOperatorAttribute)
                {
                    var op = (ExposedMathsOperatorAttribute) attr;
                    yield return new Operator(op.OperatorSymbol, op.Precedence, op.Associativity, func, op.RequiredArgumentCount, methodName);
                }
                else
                {
                    var f = (ExposedMathFunctionAttribute) attr;
                    var funcName = string.IsNullOrWhiteSpace(f.FunctionName) ? methodName : f.FunctionName;
                    yield return new StandardFunction(funcName, func, f.RequiredArgumentCount);
                }
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
    }
}