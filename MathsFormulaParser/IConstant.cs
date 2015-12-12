using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alistair.Tudor.MathsFormulaParser
{
    /// <summary>
    /// Represents a constant
    /// </summary>
    public interface IConstant
    {
        /// <summary>
        /// Gets the name of the constant
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the value of the constant
        /// </summary>
        double Value { get; }
    }
}
