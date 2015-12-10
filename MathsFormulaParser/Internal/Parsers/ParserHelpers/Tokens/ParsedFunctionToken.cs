using Alistair.Tudor.MathsFormulaParser.Internal.Functions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens
{
    /// <summary>
    /// Represents an function token
    /// </summary>
    internal class ParsedFunctionToken : ParsedToken
    {
        /// <summary>
        /// Function token represents
        /// </summary>
        public FormulaFunction Function { get; }

        public ParsedFunctionToken(FormulaFunction function, long characterPosition) : base(characterPosition)
        {
            Function = function;
        }

        /// <summary>
        /// Gets the Function string value
        /// </summary>
        public string Value => Function.FunctionName;

        /// <summary>
        /// Gets a string description of the token
        /// </summary>
        /// <returns></returns>
        protected override string ProvideValueString()
        {
            return $"Function - { Function }";
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