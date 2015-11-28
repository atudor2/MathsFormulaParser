using Alistair.Tudor.MathsFormulaParser.Internal.Operators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens
{
    /// <summary>
    /// Represents an operator token
    /// </summary>
    internal class ParsedOperatorToken : ParsedToken
    {
        /// <summary>
        /// Operator token represents
        /// </summary>
        public Operator Operator { get; private set; }

        public ParsedOperatorToken(Operator @operator)
        {
            Operator = @operator;
        }

        /// <summary>
        /// Gets the operator string value
        /// </summary>
        public string Value => Operator.OperatorSymbol;

        /// <summary>
        /// Gets a string description of the token
        /// </summary>
        /// <returns></returns>
        protected override string ProvideValueString()
        {
            return $"Operator - { Value }";
        }

        /// <summary>
        /// Gets the value of the token as a string
        /// </summary>
        /// <returns></returns>
        public override string GetStringValue()
        {
            return Value;
        }
    }
}