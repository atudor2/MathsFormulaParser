using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    /// <summary>
    /// Represents a evaluate function exception
    /// </summary>
    [Serializable]
    public class EvaluateFunctionException : CallbackFunctionException
    {
        public EvaluateFunctionException()
        {
        }

        public EvaluateFunctionException(string message) : base(message)
        {
        }

        public EvaluateFunctionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EvaluateFunctionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}