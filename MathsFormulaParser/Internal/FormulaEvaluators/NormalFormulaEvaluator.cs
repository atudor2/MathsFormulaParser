using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvaluators
{
    /// <summary>
    /// Represents a formula evaluator where formula execution is handled by a normal, stack based RPN evaluator
    /// </summary>
    internal class NormalFormulaEvaluator : IInternalFormulaEvaluator
    {
        public NormalFormulaEvaluator(ParsedToken[] rpnTokens)
        {
            RpnTokens = rpnTokens;
        }

        /// <summary>
        /// Current RPN tokens
        /// </summary>
        public ParsedToken[] RpnTokens { get; }

        /// <summary>
        /// Evaluates the formula with the given variables
        /// </summary>
        /// <param name="variableMap"></param>
        /// <returns></returns>
        public double Evaluate(IDictionary<string, double> variableMap)
        {
            var evaluator = new StandardRpnEvaluator(RpnTokens);
            return evaluator.EvaluateFormula(variableMap);
        }
    }
}
