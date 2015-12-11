using System;
using System.Runtime.Serialization;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;

namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Represents a parser failure
    /// </summary>
    [Serializable]
    public class FormulaParseException : FormulaException, IFailurePointMessageProvider
    {

        /// <summary>
        /// Position along input where failure occurred
        /// </summary>
        public long FailurePosition { get; }


        /// <summary>
        /// Tries to get a 'failure point' message showing the user where exactly an error occurred on the input
        /// </summary>
        /// <param name="input"></param>
        /// <returns>NULL if not available</returns>
        public string TryMakeFailurePointMessage(string input)
        {
            return FailurePointMessageBuilder.MakeMessage(input, Message, FailurePosition);
        }

        public FormulaParseException()
        {
        }

        public FormulaParseException(string message, long failurePosition = -1) : base(message)
        {
            FailurePosition = failurePosition;
        }

        public FormulaParseException(string message, Exception inner, long failurePosition = -1) : base(message, inner)
        {
            FailurePosition = failurePosition;
        }

        protected FormulaParseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}