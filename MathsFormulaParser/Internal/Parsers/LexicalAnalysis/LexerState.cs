namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.LexicalAnalysis
{
    /// <summary>
    /// State of the lexer
    /// </summary>
    internal enum LexerState
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal,

        /// <summary>
        /// Currently building a NUMBER
        /// </summary>
        NumberRun,

        /// <summary>
        /// Currently building a WORD
        /// </summary>
        WordRun
    }
}