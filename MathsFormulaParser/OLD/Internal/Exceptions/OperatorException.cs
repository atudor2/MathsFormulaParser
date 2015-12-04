using System;
using System.Runtime.Serialization;
using Alistair.Tudor.MathsFormulaParser.Exceptions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    [Serializable]
    internal class OperatorException : FormulaException
    {
        public OperatorException()
        {
        }

        public OperatorException(string message) : base(message)
        {
        }

        public OperatorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected OperatorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
