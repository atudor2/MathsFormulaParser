using System;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Operators
{
    /// <summary>
    /// Represents an Function
    /// </summary>
    internal class Operator : FormulaFunction
    {
        public Operator(string operatorSymbol, int precedence, OperatorAssociativity associativity, FormulaCallbackFunction callbackFunction, int requiredNumberOfArguments = 2, string functionName = null) :
                    base(operatorSymbol, callbackFunction, requiredNumberOfArguments)
        {
            operatorSymbol.ThrowIfNull(nameof(operatorSymbol));

            Precedence = precedence;
            Associativity = associativity;

            if (operatorSymbol.Length > MaxOperatorSymbolSize)
            {
                throw new ArgumentOutOfRangeException(nameof(operatorSymbol), $"Operator Symbol cannot be longer than '{MaxOperatorSymbolSize}'");
            }

            OperatorSymbol = operatorSymbol;
            UnderlyingFunctionName = GetFunctionName(operatorSymbol, functionName);
        }

        /// <summary>
        /// Gets the Associativity or the Function
        /// </summary>
        public OperatorAssociativity Associativity { get; private set; }

        /// <summary>
        /// Gets whether the operator is unary. This is determined by: Is it Right Associative and has it only got 1 argument?
        /// </summary>
        public bool IsUnaryOperator => Associativity == OperatorAssociativity.Right && RequiredNumberOfArguments == 1;

        /// <summary>
        /// Gets the string representing the Function symbol (e.g. function name)
        /// <remarks>CAN ONLY BE 'MaxOperatorSymbolSize' CHARACTERS</remarks>
        /// </summary>
        public string OperatorSymbol { get; }


        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public override string FunctionName => OperatorSymbol;

        /// <summary>
        /// Gets the underlying function name implementing the Function
        /// </summary>
        public string UnderlyingFunctionName { get; }

        /// <summary>
        /// Gets the Function's precedence
        /// </summary>
        public int Precedence { get; private set; }

        /// <summary>
        /// Maximum Function symbol length
        /// </summary>
        internal static int MaxOperatorSymbolSize => SpecialConstants.MaxOperatorSymbolSize;

        /// <summary>
        /// Returns the Function symbol or function name depending on if function name is null/whitespace
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        private static string GetFunctionName(string symbol, string functionName)
        {
            return string.IsNullOrWhiteSpace(functionName) ? symbol : functionName;
        }
    }
}
