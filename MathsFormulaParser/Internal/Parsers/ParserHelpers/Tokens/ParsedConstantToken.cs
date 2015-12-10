namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens
{
    /// <summary>
    /// Represents a constant token
    /// </summary>
    internal class ParsedConstantToken : ParsedValueToken
    {
        /// <summary>
        /// Gets the name of the constant
        /// </summary>
        public string Name { get; }

        public ParsedConstantToken(string name, double value, long characterPosition) : base(value, characterPosition)
        {
            Name = name;
        }

        /// <summary>
        /// Gets a string description of the token
        /// </summary>
        /// <returns></returns>
        protected override string ProvideValueString()
        {
            return $"Constant - { Name } = { Value }";
        }
    }
}