using System;
using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Functions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols
{
    internal class FunctionComparer : IEqualityComparer<StandardFunction>
    {
        public bool Equals(StandardFunction x, StandardFunction y)
        {
            //Check whether the compared objects reference the same data.
            if (object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;

            return string.Equals(x.FunctionName, y.FunctionName, StringComparison.InvariantCultureIgnoreCase);
        }
        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(StandardFunction func)
        {
            //Check whether the object is null
            if (object.ReferenceEquals(func, null)) return 0;

            return func.GetHashCode();
        }
    }
}
