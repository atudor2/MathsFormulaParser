using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions
{
    internal static class TokenExtensions
    {
        public static string GetFormulaString(this IEnumerable<ParsedToken> tokens)
        {
            tokens.ThrowIfNull(nameof(tokens));

            return string.Join(" ", tokens.Select(t => t.GetStringValue()));
        }

        public static string ToInfixNotationString(this IEnumerable<ParsedToken> tokens)
        {
            tokens.ThrowIfNull(nameof(tokens));

            // Convert the tokens to Infix notation:
            foreach (var token in tokens)
            {
                
            }

            return "!";
        }
    }
}
