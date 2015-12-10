using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Represents an evaluation failure
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : FormulaException
    {
        public FormulaEvaluationException()
        {
        }

        public FormulaEvaluationException(string message, string longMessage = null) : base(message, longMessage)
        {
        }

        public FormulaEvaluationException(string message, Exception inner, string longMessage = null) : base(message, inner, longMessage)
        {
        }

        protected FormulaEvaluationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}