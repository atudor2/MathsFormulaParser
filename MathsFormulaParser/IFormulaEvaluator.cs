using System.Collections.Generic;

namespace Alistair.Tudor.MathsFormulaParser
{
    /// <summary>
    /// Represents a basic Formula
    /// </summary>
    public interface IFormulaEvaluator
    {
        /// <summary>
        /// Gets the original formula string
        /// </summary>
        string OriginalFormula { get; }

        /// <summary>
        /// Gets the parsed formula
        /// </summary>
        string ParsedFormula { get; }

        /// <summary>
        /// Optimises the parsed formula
        /// </summary>
        void OptimiseFormula();

        /// <summary>
        /// Should extended checks be performed before evaluation?
        /// </summary>
        bool PerformExtendedChecks { get; set; }

        /// <summary>
        /// Executes the formula and returns the result
        /// </summary>
        /// <returns></returns>
        double GetResult();

        /// <summary>
        /// Clears any preset variables
        /// </summary>
        void ClearVariables();

        /// <summary>
        /// Sets the map of variables
        /// </summary>
        /// <param name="variableMap"></param>
        void SetVariableMap(IDictionary<string, double> variableMap);
    }
}
