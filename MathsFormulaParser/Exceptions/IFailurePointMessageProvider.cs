namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Marks an exception as being able to potentially provide a failure point message
    /// </summary>
    public interface IFailurePointMessageProvider
    {
        /// <summary>
        /// Position along input where failure occurred
        /// </summary>
        long FailurePosition { get; }

        /// <summary>
        /// Tries to make a 'failure point' message showing the user where exactly an error occurred on the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns>NULL if not available</returns>
        string TryMakeFailurePointMessage(string input);
    }
}
