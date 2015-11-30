using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.Evaluators;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers
{
    /// <summary>
    /// Standard Formula Evaluator
    /// </summary>
    internal class FormulaEvaluator : IFormulaEvaluator
    {
        private Dictionary<string, double> _variableMap = new Dictionary<string, double>();
        public ParsedToken[] RpnTokens { get; private set; }

        public FormulaEvaluator(ParsedToken[] rpnTokens, string originalFormula)
        {
            RpnTokens = rpnTokens;
            OriginalFormula = originalFormula;
        }

        /// <summary>
        /// Gets the original formula string
        /// </summary>
        public string OriginalFormula { get; }

        /// <summary>
        /// Gets the parsed formula
        /// </summary>
        public string ParsedFormula => RpnTokens.ToInfixNotationString();

        /// <summary>
        /// Gets the parsed formula in raw form
        /// </summary>
        public string RawParsedFormula => RpnTokens.GetFormulaString();

        /// <summary>
        /// Optimises the parsed formula
        /// </summary>
        public void OptimiseFormula()
        {
            var rpnOptimiser = new RpnOptimiser(RpnTokens);
            RpnTokens = rpnOptimiser.OptimiseExpression();
        }

        /// <summary>
        /// Should extended checks be performed before evaluation?
        /// </summary>
        public bool PerformExtendedChecks { get; set; }

        /// <summary>
        /// Executes the formula and returns the result
        /// </summary>
        /// <returns></returns>
        public double GetResult()
        {
            var evaluator = new StandardRpnEvaluator(RpnTokens) { PerformExtendedChecks = PerformExtendedChecks };
            return evaluator.EvaluateFormula(_variableMap);
        }

        /// <summary>
        /// Clears any preset variables
        /// </summary>
        public void ClearVariables()
        {
            _variableMap.Clear();
        }

        /// <summary>
        /// Sets the map of variables
        /// </summary>
        /// <param name="variableMap"></param>
        public void SetVariableMap(IDictionary<string, double> variableMap)
        {
            _variableMap = new Dictionary<string, double>(variableMap);
        }
    }
}
