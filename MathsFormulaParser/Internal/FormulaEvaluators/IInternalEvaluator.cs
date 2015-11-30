using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;

namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvaluators
{
    internal interface IInternalEvaluator
    {
        /// <summary>
        /// Gets the parsed tokens
        /// </summary>
        /// <returns></returns>
        ParsedToken[] Tokens { get; }

        /// <summary>
        /// Evaluates the formula
        /// </summary>
        /// <param name="variableMap"></param>
        /// <param name="extendedChecks"></param>
        /// <returns></returns>
        double Evaluate(IDictionary<string, double> variableMap, bool extendedChecks);
    }
}
