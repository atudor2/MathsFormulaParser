using System;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.LexicalAnalysis
{
    /// <summary>
    /// Represents a lexical token
    /// </summary>
    internal class LexicalToken
    {
        public LexicalToken(LexicalTokenType tokenType, string value, long characterPosition = -1)
        {
            TokenType = tokenType;
            Value = value;
            CharacterPosition = characterPosition;
        }

        /// <summary>
        /// Position of token on original input string. (if &lt; 0 - position is not available)
        /// </summary>
        public long CharacterPosition { get; }

        /// <summary>
        /// Token type
        /// </summary>
        public LexicalTokenType TokenType { get; }

        /// <summary>
        /// Value of the token (if not available - null)
        /// </summary>
        public string? Value { get; }

        /// <summary>
        /// Gets the Token type as a string
        /// </summary>
        /// <returns></returns>
        public string GetTypeAsName()
        {
            return Enum.GetName(typeof (LexicalTokenType), TokenType) ?? throw new InvalidOperationException("Cannot determine LexicalTokenType name");
        }

        public override string ToString()
        {
            return $"{GetTypeAsName()} - {Value ?? "<null>" }";
        }
    }
}
