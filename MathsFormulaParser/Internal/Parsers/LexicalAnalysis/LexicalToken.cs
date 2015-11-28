using System;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.LexicalAnalysis
{
    /// <summary>
    /// Represents a lexical token
    /// </summary>
    internal class LexicalToken
    {
        /// <summary>
        /// Token type
        /// </summary>
        public LexicalTokenType TokenType { get; private set; }

        /// <summary>
        /// Value of the token (if not available - null)
        /// </summary>
        public string Value { get; private set; }

        public LexicalToken(LexicalTokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        /// <summary>
        /// Gets the Token type as a string
        /// </summary>
        /// <returns></returns>
        public string GetTypeAsName()
        {
            return Enum.GetName(typeof (LexicalTokenType), TokenType);
        }

        public override string ToString()
        {
            return $"{GetTypeAsName()} - {Value ?? "<null>" }";
        }
    }
}
