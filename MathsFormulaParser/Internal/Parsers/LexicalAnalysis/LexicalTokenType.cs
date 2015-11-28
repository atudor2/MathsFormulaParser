namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers.LexicalAnalysis
{
    /// <summary>
    /// Lexical token types
    /// </summary>
    internal enum LexicalTokenType
    {
        StartSubExpression,
        EndSubExpression,
        Operator,
        StartSubScript,
        EndSubScript,
        Number,
        Comma,
        Word,
        Space
    }
}
