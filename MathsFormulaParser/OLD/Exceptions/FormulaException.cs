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
        public FormulaException()
        {
        }

        public FormulaException(string message) : base(message)
        {
        }

        public FormulaException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FormulaException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
