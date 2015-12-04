using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvaluators
{
    /// <summary>
    /// Internal class for normal, stack based RPN evaluation
    /// </summary>
    internal class NormalTokenEvaluator : IInternalEvaluator
    {
        public NormalTokenEvaluator(ParsedToken[] tokens)
        {
            Tokens = tokens;
        }

        /// <summary>
        /// Gets the parsed tokens
        /// </summary>
        /// <returns></returns>
        public ParsedToken[] Tokens { get; }

        /// <summary>
        /// Evaluates the formula
        /// </summary>
        /// <param name="variableMap"></param>
        /// <param name="performExtendedChecks"></param>
        /// <returns></returns>
        public double Evaluate(IDictionary<string, double> variableMap, bool performExtendedChecks)
        {
            var evaluator = new StandardRpnEvaluator(Tokens) { PerformExtendedChecks = performExtendedChecks };
            return evaluator.EvaluateFormula(variableMap);
        }
    }
}
