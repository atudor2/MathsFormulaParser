using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    /// <summary>
    /// Represents a callback function exception
    /// </summary>
    [Serializable]
    internal class CallbackFunctionException : BaseInternalFormulaException
    {
       public CallbackFunctionException()
        {
        }

        public CallbackFunctionException(string message) : base(message)
        {
        }

        public CallbackFunctionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CallbackFunctionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
