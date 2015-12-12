using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.Helpers
{
    /// <summary>
    /// Represents a class that can resolve a variable
    /// </summary>
    internal interface IIVariableResolver
    {
        /// <summary>
        /// Resolves a given variable. Throws an exception if variable is not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        double ResolveVariable(string name);
    }
}
