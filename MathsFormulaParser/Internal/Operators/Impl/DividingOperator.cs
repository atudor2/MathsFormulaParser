using System;
using System.Linq;
using Alistair.Tudor.MathsFormulaParser.Internal.Exceptions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Operators.Impl
{
    /// <summary>
    /// Operator that represents an operator that divides
    /// <remarks>As part of the extended check, it will check that the divisor is not 0</remarks>
    /// </summary>
    internal class DividingOperator : GenericOperator
    {
        public DividingOperator(int precedence, string operatorSymbol, OperatorAssociativity associativity, int requiredNumberOfArguments, FormulaCallbackFunction evaluateFunc) : 
            base(precedence, operatorSymbol, associativity, requiredNumberOfArguments, evaluateFunc)
        {
        }

        /// <summary>
        /// Internal method for implementing extended input checks
        /// </summary>
        /// <param name="input"></param>
        protected override void InternalCheckInput(double[] input)
        {
            var divisor = input.Last();
            // TODO: Implement safer double comparison with a tolerance vs straight compare
            if (divisor == 0)
            {
                // Cannot divide by 0!
                throw new OperatorExtendedCheckException("Cannot divide by zero");
            }
            
        }
    }
}
