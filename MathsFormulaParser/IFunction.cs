namespace Alistair.Tudor.MathsFormulaParser
{
    /// <summary>
    /// Represents a Function object
    /// </summary>
    public interface IFunction
    {
        /// <summary>
        /// Gets the name of the function
        /// </summary>
        string FunctionName { get; }

        /// <summary>
        /// Gets the required argument count
        /// </summary>
        int RequiredNumberOfArguments { get; }

        /// <summary>
        /// Returns a 'pretty' string form of the function - e.g. f(a, b)
        /// </summary>
        /// <returns></returns>
        string GetPrettyFunctionString();
    }
}
