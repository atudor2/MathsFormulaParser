using System;
using System.Collections.Generic;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Functions
{
    internal class FunctionComparer : IEqualityComparer<Function>
    {
        public bool Equals(Function x, Function y)
        {
            //Check whether the compared objects reference the same data.
            if (object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;

            return string.Equals(x.FunctionName, y.FunctionName, StringComparison.InvariantCultureIgnoreCase);
        }
        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(Function func)
        {
            //Check whether the object is null
            if (object.ReferenceEquals(func, null)) return 0;

            return func.GetHashCode();
        }
    }
}
