﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        /// RPN Token backing field
        /// </summary>
        private ParsedToken[] _rpnTokens;

        /// <summary>
        /// Variable map
        /// </summary>
        private Dictionary<string, double> _variableMap = new Dictionary<string, double>();
        /// <summary>
        /// Variables required for expression
        /// </summary>
        private string[] _varsRequired = null;

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
        /// Gets the list of required variables or NULL
        /// </summary>
        public IReadOnlyList<string> RequiredVariables => _varsRequired;

        /// <summary>
        /// Returns TRUE if variables are required for the expression
        /// </summary>
        public bool RequiresVariables => this.RequiredVariables.Any();

        /// <summary>
        /// Parsed Tokens
        /// </summary>
        public ParsedToken[] RpnTokens
        {
            get { return _rpnTokens; }
            private set
            {
                _rpnTokens = value;
                UpdateVariableRequirementsList();
            }
        }

        /// <summary>
        /// Clears any preset variables
        /// </summary>
        public void ClearVariables()
        {
            _variableMap.Clear();
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
        /// Optimises the parsed formula using the BASIC optimisation level
        /// </summary>
        public void OptimiseFormula()
        {
            var optimiser = new RpnOptimiser(RpnTokens);
            RpnTokens = optimiser.OptimiseExpression();
        }

        /// <summary>
        /// Sets the map of variables
        /// </summary>
        /// <param name="variableMap"></param>
        public void SetVariableMap(IDictionary<string, double> variableMap)
        {
            _variableMap = new Dictionary<string, double>(variableMap);
        }

        /// <summary>
        /// Updates the internal variables required list
        /// </summary>
        private void UpdateVariableRequirementsList()
        {
            _varsRequired = RpnTokens.OfType<ParsedVariableToken>().Select(v => v.Name).ToArray();
        }
    }
}
