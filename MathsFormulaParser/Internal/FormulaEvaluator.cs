using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Internal evaluator
        /// </summary>
        private IInternalFormulaEvaluator _internalFormulaEvaluator;

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
            var evaluator = _internalFormulaEvaluator ?? FormulaOptimiser.NoOptimisation(RpnTokens);
            return evaluator.Evaluate(_variableMap);
        }

        /// <summary>
        /// Optimises the parsed formula using the BASIC optimisation level
        /// </summary>
        public void OptimiseFormula()
        {
            OptimiseFormula(FormulaOptimisationLevel.Basic);
        }

        /// <summary>
        /// Optimises the parsed formula
        /// </summary>
        public void OptimiseFormula(FormulaOptimisationLevel level)
        {
            switch (level)
            {
                case FormulaOptimisationLevel.None:
                    // Nothing!
                    return;
                case FormulaOptimisationLevel.Basic:
                    _internalFormulaEvaluator = FormulaOptimiser.BasicOptimisation(RpnTokens);
                    break;
                case FormulaOptimisationLevel.Compiled:
                    _internalFormulaEvaluator = FormulaOptimiser.CompiledOptimisation(RpnTokens);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
            RpnTokens = _internalFormulaEvaluator.RpnTokens;
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
            _varsRequired = RpnTokens.OfType<ParsedVariableToken>().Select(v => v.Name).Distinct().ToArray();
        }
    }
}
