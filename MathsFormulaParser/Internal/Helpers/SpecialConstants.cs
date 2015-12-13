namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    /// <summary>
    /// Holder for special constants used when parsing or executing a formula
    /// </summary>
    internal static class SpecialConstants
    {
        /// <summary>
        /// Bit index operator symbol
        /// Used when parsing a bit operator (x[y])
        /// </summary>
        public const string GetBitOperatorSymbol = "!@";

        /// <summary>
        /// Symbol for subexpression start
        /// </summary>
        public const string SubExpressionStart = "(";

        /// <summary>
        /// Symbol for subexpression end
        /// </summary>
        public const string SubExpressionEnd = ")";

        /// <summary>
        /// Maximum allowed character length for operator symbols
        /// </summary>
        public const int MaxOperatorSymbolSize = 2;
    }
}
