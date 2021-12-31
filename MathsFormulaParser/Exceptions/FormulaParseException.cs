using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Represents a parser failure
    /// </summary>
    [Serializable]
    public class FormulaParseException : FormulaProcessingException
    {
        public FormulaParseException()
        {
        }

        public FormulaParseException(string message, long failurePosition = -1, string additionalInfo = "") : base(message, failurePosition, additionalInfo)
        {
        }

        public FormulaParseException(string message, Exception inner, long failurePosition = -1, string additionalInfo = "") : base(message, inner, failurePosition, additionalInfo)
        {
        }

        protected FormulaParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}