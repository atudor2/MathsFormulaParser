namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvaluators.Helpers
{
    /// <summary>
    /// Represents a class that can resolve a variable
    /// </summary>
    internal interface IVariableResolver
    {
        /// <summary>
        /// Resolves a given variable. Throws an exception if variable is not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        double ResolveVariable(string name);
    }
}
