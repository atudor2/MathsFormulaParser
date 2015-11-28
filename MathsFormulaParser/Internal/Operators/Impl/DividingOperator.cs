using System;
using System.Linq;
using Alistair.Tudor.MathsFormulaParser.Internal.Exceptions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Operators.Impl
{
    /// <summary>
    /// Operator that represents an operator that divides
    /// <remarks>As part of the extended check, it will check that the divisor is not 0</remarks>
    /// </summary>
    internal class DividingOperator : Operator
    {
        private readonly FormulaCallbackFunction _evaluateFunc;

        public DividingOperator(int precedence, string operatorSymbol, OperatorAssociativity associativity, int requiredNumberOfArguments, FormulaCallbackFunction evaluateFunc) : 
            base(precedence, operatorSymbol, associativity, requiredNumberOfArguments)
        {
            _evaluateFunc = evaluateFunc;
        }

        /// <summary>
        /// Internal method for evaluating the operator. Run after input checks have been executed
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override double InternalEvaluate(double[] input)
        {
            return _evaluateFunc(input);
        }

        /// <summary>
        /// Internal method for implementing extended input checks
        /// </summary>
        /// <param name="input"></param>
        protected override void InternalCheckInput(double[] input)
        {
            var divisor = input.Last();
            if (divisor == 0)
            {
                // Cannot divide by 0!
                throw new OperatorExtendedCheckException("Cannot divide by zero");
            }
            
        }
    }
}
