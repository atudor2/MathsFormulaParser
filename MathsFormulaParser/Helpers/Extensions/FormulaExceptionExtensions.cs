using Alistair.Tudor.MathsFormulaParser.Exceptions;

namespace Alistair.Tudor.MathsFormulaParser.Helpers.Extensions
{
    /// <summary>
    /// Helper class for FormulaException Extensions
    /// </summary>
    public static class FormulaExceptionExtensions
    {
        /// <summary>
        /// Tries to get a Failure Point detailed message from the exception or NULL
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string? TryGetFailurePointMessage(this FormulaException exception, string input)
        {
            var msgHandler = exception as IFailurePointMessageProvider;
            return msgHandler?.TryMakeFailurePointMessage(input);
        }
    }
}
