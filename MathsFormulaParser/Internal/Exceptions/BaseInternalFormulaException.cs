using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    /// <summary>
    /// Base class for all internal formula parser exceptions
    /// </summary>
    [Serializable]
    internal abstract class BaseInternalFormulaException : Exception
    {
        protected BaseInternalFormulaException()
        {
        }

        protected BaseInternalFormulaException(string message) : base(message)
        {
        }

        protected BaseInternalFormulaException(string message, Exception inner) : base(message, inner)
        {
        }

        protected BaseInternalFormulaException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
