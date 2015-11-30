namespace Alistair.Tudor.MathsFormulaParser
{
    /// <summary>
    /// Level of optimisation for optimising formulaw  
    /// </summary>
    public enum FormulaOptimisationLevel
    {
        /// <summary>
        /// No optimisation is done
        /// </summary>
        None, 

        /// <summary>
        /// Basic optimisation: Flattens constant expressions
        /// </summary>
        Basic,

        /// <summary>
        /// Performs basic optimisation and attempts to generate direct function calls.
        /// NB: Extended Checks are NOT usable when compiled!
        /// </summary>
        /// <remarks>NOT STABLE!</remarks>
        Compiled
    }
}
