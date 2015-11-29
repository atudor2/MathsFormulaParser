using System;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Operators.Impl
{
    /// <summary>
    /// Represents a standard Operator that simply evaluates it's arguments without any additional checks available
    /// </summary>
    internal class GenericOperator : Operator
    {
        /// <summary>
        /// Function pointer to evaluate the operator 
        /// </summary>
        private readonly FormulaCallbackFunction _evaluateFunc;

        public GenericOperator(int precedence, string operatorSymbol, OperatorAssociativity associativity, int requiredNumberOfArguments, FormulaCallbackFunction evaluateFunc) : 
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
    }
}
