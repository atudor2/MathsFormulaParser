using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Represents a callback function exception
    /// </summary>
    [Serializable]
    internal class FormulaCallbackFunctionException : FormulaException
    {
       public FormulaCallbackFunctionException()
        {
        }

        public FormulaCallbackFunctionException(string message) : base(message)
        {
        }

        public FormulaCallbackFunctionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FormulaCallbackFunctionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
