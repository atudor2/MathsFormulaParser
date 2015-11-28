using System.Globalization;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens
{
    /// <summary>
    /// Represents the base class for tokens tha provide a numerical value
    /// </summary>
    internal abstract class ParsedValueToken : ParsedToken
    {
        /// <summary>
        /// Gets the token value
        /// </summary>
        public double Value { get; protected set; }

        protected ParsedValueToken(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value of the token as a string
        /// </summary>
        /// <returns></returns>
        public override string GetStringValue()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}