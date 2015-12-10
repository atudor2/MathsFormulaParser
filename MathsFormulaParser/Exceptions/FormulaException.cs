using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Top level class for Formula exceptions
    /// </summary>
    [Serializable]
    public class FormulaException : Exception
    {
        /// <summary>
        /// Gets the long message for the error if available or null
        /// </summary>
        public string LongMessage { get; }

        public FormulaException()
        {
        }

        public FormulaException(string message, string longMessage = null) : base(message)
        {
            LongMessage = longMessage;
        }

        public FormulaException(string message, Exception inner, string longMessage = null) : base(message, inner)
        {
            LongMessage = longMessage;
        }

        protected FormulaException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
