using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    /// <summary>
    /// Holder for special constants used when parsing or executing a formula
    /// </summary>
    internal static class SpecialConstants
    {
        /// <summary>
        /// Bit index operator symbol
        /// Used when parsing a bit operator (x[y])
        /// </summary>
        public const string GetBitOperatorSymbol = "!@";
    }
}
