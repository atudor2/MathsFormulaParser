using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Evaluators
{
    /// <summary>
    /// Standard Reverse Polish Notation Evaluator
    /// </summary>
    internal class StandardRpnEvaluator : AbstractRpnEvaluator
    {
        /// <summary>
        /// Current dictionary of variables
        /// </summary>
        private IDictionary<string, double> _currentVarMap;

        public StandardRpnEvaluator(ParsedToken[] rpnTokens) : base(rpnTokens)
        {
        }

        /// <summary>
        /// Resolves a variable name to its value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override double ResolveVariable(string name)
        {
            double varValue;
            if (!_currentVarMap.TryGetValue(name.ToUpper(), out varValue))
            {
                RaiseError(null, $"Cannot find variable '{ name }''");
            }
            return varValue;
        }

        /// <summary>
        /// Evaluates the given formula with the given variable map
        /// </summary>
        /// <param name="variableMap"></param>
        /// <returns></returns>
        public double EvaluateFormula(IDictionary<string, double> variableMap)
        {
            try
            {
                _currentVarMap = variableMap;

                this.Reset();

                while (this.HasTokens)
                {
                    this.ReadNextToken();
                }

                return this.GetResult();
            }
            finally
            {
                _currentVarMap = null; // Always reset the var map
            }
        }
    }
}
