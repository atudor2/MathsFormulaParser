using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    /// <summary>
    /// RPN Evaluation Exception
    /// </summary>
    [Serializable]
    public class RpnEvaluationException : Exception
    {
        public RpnEvaluationException()
        {
        }

        public RpnEvaluationException(string message) : base(message)
        {
        }

        public RpnEvaluationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RpnEvaluationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}