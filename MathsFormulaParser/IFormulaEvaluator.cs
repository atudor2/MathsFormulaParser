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
        /// Returns TRUE if variables are required for the expression
        /// </summary>
        bool RequiresVariables { get; }

        /// <summary>
        /// Gets the list of required variables or NULL
        /// </summary>
        IReadOnlyList<string> RequiredVariables { get; }

        /// <summary>
        /// Gets the parsed formula
        /// </summary>
        string ParsedFormula { get; }

        /// <summary>
        /// Gets the parsed formula in raw form
        /// </summary>
        string RawParsedFormula { get; }

        /// <summary>
        /// Optimises the parsed formula with BASIC optimisation
        /// </summary>
        void OptimiseFormula();

        /// <summary>
        /// Optimises the parsed formula
        /// </summary>
        void OptimiseFormula(FormulaOptimisationLevel level);

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
