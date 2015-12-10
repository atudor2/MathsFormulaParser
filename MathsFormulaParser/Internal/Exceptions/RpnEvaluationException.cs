using System;
using System.Runtime.Serialization;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    /// <summary>
    /// RPN Evaluation Exception
    /// </summary>
    [Serializable]
    internal class RpnEvaluationException : BaseInternalFormulaException, IFailurePointMessageProvider
    {
        /// <summary>
        /// Position along input where failure occurred
        /// </summary>
        public long FailurePosition { get; }


        /// <summary>
        /// Tries to create a 'failure point' message showing the user where exactly an error occurred on the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string TryMakeFailurePointMessage(string input)
        {
            return FailurePointMessageBuilder.MakeMessage(input, Message, FailurePosition);
        }

        public RpnEvaluationException()
        {
        }

        public RpnEvaluationException(string message, long failurePosition) : base(message)
        {
        }

        public RpnEvaluationException(string message, Exception inner, long failurePosition) : base(message, inner)
        {
        }

        protected RpnEvaluationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}