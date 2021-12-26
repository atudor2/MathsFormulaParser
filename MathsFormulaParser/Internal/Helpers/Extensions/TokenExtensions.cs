using System;
using System.Collections.Generic;
using System.Linq;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions
{
    /// <summary>
    /// Extension for ParsedTokens
    /// </summary>
    internal static class TokenExtensions
    {
        /// <summary>
        /// Converts the given tokens to a string representation (Reverse Polish Notation)
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static string GetFormulaString(this IEnumerable<ParsedToken> tokens)
        {
            ArgumentNullException.ThrowIfNull(tokens);

            return string.Join(" ", tokens.Select(t => t.GetStringValue()));
        }

        /// <summary>
        /// Converts the given tokens to a string in INFIX notation
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static string ToInfixNotationString(this IEnumerable<ParsedToken> tokens)
        {
            ArgumentNullException.ThrowIfNull(tokens);

            // Convert the tokens to Infix notation:
            // Use the special class:
            var infixEval = new InfixNotationRpnEvaluator(tokens.ToArray());
            return infixEval.GetInfixForm();
        }
    }
}
