using System;
using System.Runtime.Serialization;
using Alistair.Tudor.MathsFormulaParser.Exceptions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    /// <summary>
    /// Exception raised when a Reverse Polish Notation formula encounters an error
    /// </summary>
    [Serializable]
    public class RpnEvaluationException : FormulaException
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
