namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens
{
    /// <summary>
    /// Represents a number literal token
    /// </summary>
    internal class ParsedNumberToken : ParsedValueToken
    {
        public ParsedNumberToken(double value) : base(value)
        {
        }

        /// <summary>
        /// Gets a string description of the token
        /// </summary>
        /// <returns></returns>
        protected override string ProvideValueString()
        {
            return $"Number - { Value }";
        }
    }
}