using System;
using System.Runtime.Serialization;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Exceptions
{
    [Serializable]
    internal class OperatorExtendedCheckException : OperatorException
    {
        public OperatorExtendedCheckException()
        {
        }

        public OperatorExtendedCheckException(string message) : base(message)
        {
        }

        public OperatorExtendedCheckException(string message, Exception inner) : base(message, inner)
        {
        }

        protected OperatorExtendedCheckException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}