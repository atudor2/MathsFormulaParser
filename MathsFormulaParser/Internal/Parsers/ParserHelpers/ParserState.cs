namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers
{
    /// <summary>
    /// State of the parser
    /// </summary>
    internal enum ParserState
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal,

        /// <summary>
        /// Within a subscript
        /// </summary>
        InSubScript
    }
}
