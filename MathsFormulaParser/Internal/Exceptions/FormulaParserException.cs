using System;
using System.Runtime.Serialization;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    /// <summary>
    /// Represents a formula parser exception
    /// </summary>
    [Serializable]
    internal class FormulaParserException : BaseInternalFormulaException, IFailurePointMessageProvider
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

        public FormulaParserException()
        {
        }

        public FormulaParserException(string message, long failurePosition = -1) : base(message)
        {
            FailurePosition = failurePosition;
        }

        public FormulaParserException(string message, Exception inner, long failurePosition = -1) : base(message, inner)
        {
            FailurePosition = failurePosition;
        }

        protected FormulaParserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
