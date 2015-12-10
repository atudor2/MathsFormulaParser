using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Represents a parser failure
    /// </summary>
    [Serializable]
    public class FormulaParseException : FormulaException
    {
        public FormulaParseException()
        {
        }

        public FormulaParseException(string message, string longMessage = null) : base(message, longMessage)
        {
        }

        public FormulaParseException(string message, Exception inner, string longMessage = null) : base(message, inner, longMessage)
        {
        }

        protected FormulaParseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}