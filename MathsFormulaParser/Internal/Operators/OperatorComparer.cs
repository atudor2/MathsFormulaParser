using System;
using System.Collections.Generic;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Operators
{
    internal class OperatorComparer : IEqualityComparer<Operator>
    {
        public bool Equals(Operator x, Operator y)
        {
            //Check whether the compared objects reference the same data.
            if (object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;

            return string.Equals(x.OperatorSymbol, y.OperatorSymbol, StringComparison.InvariantCultureIgnoreCase);
        }
        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(Operator op)
        {
            //Check whether the object is null
            if (object.ReferenceEquals(op, null)) return 0;

            return op.GetHashCode();
        }
    }
}
