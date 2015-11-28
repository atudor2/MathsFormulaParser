namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens
{
    /// <summary>
    /// Represents a Variable Token
    /// </summary>
    internal class ParsedVariableToken : ParsedToken
    {
        /// <summary>
        /// Gets the variable name
        /// </summary>
        public string Name { get; private set; }

        public ParsedVariableToken(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets a string description of the token
        /// </summary>
        /// <returns></returns>
        protected override string ProvideValueString()
        {
            return $"Variable - { Name }";
        }

        /// <summary>
        /// Gets the value of the token as a string
        /// </summary>
        /// <returns></returns>
        public override string GetStringValue()
        {
            return Name;
        }

    }
}