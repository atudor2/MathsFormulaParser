namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens
{
    /// <summary>
    /// Represents a basic parsed token
    /// </summary>
    internal abstract class ParsedToken
    {
        /// <summary>
        /// Gets a string description of the token
        /// </summary>
        /// <returns></returns>
        protected abstract string ProvideValueString();

        /// <summary>
        /// Gets the value of the token as a string
        /// </summary>
        /// <returns></returns>
        public abstract string GetStringValue();

        /// <summary>
        /// Returns a string that represents the current token.
        /// </summary>
        /// <returns>
        /// A string that represents the current token.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return ProvideValueString();
        }
    }
}
