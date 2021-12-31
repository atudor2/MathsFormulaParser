using System;
using System.Runtime.Serialization;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;

namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Represents a general formula processing failure
    /// Will contain a pointer to the section of formula that failed
    /// </summary>
    [Serializable]
    public class FormulaProcessingException : FormulaException, IFailurePointMessageProvider
    {
        private readonly string _additionalInfo;

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
            return FailurePointMessageBuilder.MakeMessage(input, Message, FailurePosition, _additionalInfo);
        }

        public FormulaProcessingException()
        {
        }

        public FormulaProcessingException(string message, long failurePosition = -1, string additionalInfo = "") :
            base(message)
        {
            _additionalInfo = additionalInfo;
            FailurePosition = failurePosition;
        }

        public FormulaProcessingException(string message, Exception inner, long failurePosition = -1,
            string additionalInfo = "") : base(message, inner)
        {
            FailurePosition = failurePosition;
            _additionalInfo = additionalInfo;
        }

        protected FormulaProcessingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}