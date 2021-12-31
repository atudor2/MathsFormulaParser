using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Exceptions
{
    /// <summary>
    /// Represents an evaluation failure
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : FormulaProcessingException
    {
        public FormulaEvaluationException()
        {
        }

        public FormulaEvaluationException(string message, long failurePosition = -1, string additionalInfo = "") : base(message, failurePosition, additionalInfo)
        {
        }

        public FormulaEvaluationException(string message, Exception inner, long failurePosition = -1, string additionalInfo = "") : base(message, inner, failurePosition, additionalInfo)
        {
        }

        protected FormulaEvaluationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}