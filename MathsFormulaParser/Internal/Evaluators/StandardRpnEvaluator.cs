using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Evaluators
{
    /// <summary>
    /// Standard Reverse Polish Notation Evaluator
    /// </summary>
    internal class StandardRpnEvaluator : AbstractRpnEvaluator
    {
        private readonly Dictionary<string, double> _emptyDic = new Dictionary<string, double>();
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

        protected override bool OnOperatorToken(ParsedOperatorToken token)
        {
            token.Operator.UseExtendedInputChecks = PerformExtendedChecks;
            return base.OnOperatorToken(token);
        }

        public bool PerformExtendedChecks { get; set; }

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
