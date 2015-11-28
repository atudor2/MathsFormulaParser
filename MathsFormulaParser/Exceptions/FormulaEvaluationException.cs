using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Represents an evaluation failure
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : FormatException
    {
        public FormulaEvaluationException()
        {
        }

        public FormulaEvaluationException(string message) : base(message)
        {
        }

        public FormulaEvaluationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FormulaEvaluationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}