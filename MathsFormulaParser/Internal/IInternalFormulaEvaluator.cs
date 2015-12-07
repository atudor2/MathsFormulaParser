using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;

namespace Alistair.Tudor.MathsFormulaParser.Internal
{
    /// <summary>
    /// Internal Formula Evaluator - handles the actual Evaluation of a formula
    /// </summary>
    internal interface IInternalFormulaEvaluator
    {
        /// <summary>
        /// Current RPN tokens
        /// </summary>
        ParsedToken[] RpnTokens { get; }

        /// <summary>
        /// Evaluates the formula with the given variables
        /// </summary>
        /// <param name="variableMap"></param>
        /// <returns></returns>
        double Evaluate(IDictionary<string, double> variableMap);
    }
}
