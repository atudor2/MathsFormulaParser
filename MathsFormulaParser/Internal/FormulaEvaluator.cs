using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators;

namespace Alistair.Tudor.MathsFormulaParser.Internal
{
    /// <summary>
    /// Standard Formula Evaluator
    /// </summary>
    internal class FormulaEvaluator : IFormulaEvaluator
    {
        /// <summary>
        /// Variable map
        /// </summary>
        private Dictionary<string, double> _variableMap = new Dictionary<string, double>();

        /// <summary>
        /// Parsed Tokens
        /// </summary>
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
        /// Optimises the parsed formula using the BASIC optimisation level
        /// </summary>
        public void OptimiseFormula()
        {
            var optimiser = new RpnOptimiser(RpnTokens);
            RpnTokens = optimiser.OptimiseExpression();
        }

        /// <summary>
        /// Executes the formula and returns the result
        /// </summary>
        /// <returns></returns>
        public double GetResult()
        {
            var evaluator = new StandardRpnEvaluator(RpnTokens);
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
