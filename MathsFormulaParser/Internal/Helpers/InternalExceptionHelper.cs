using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alistair.Tudor.MathsFormulaParser.Exceptions;
using Alistair.Tudor.MathsFormulaParser.Internal.Exceptions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    /// <summary>
    /// Helper class to manage internal exceptions being thrown up from the internal parsers
    /// Generally, this class will take an Exception of type BaseInternalFormulaException
    /// and convert it to a more general public exception
    /// </summary>
    internal static class InternalExceptionHelper
    {
        /// <summary>
        /// Converts an internal exception to a public wrapper exception
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static FormulaException MapInternalException(BaseInternalFormulaException ex, string input)
        {
            // Here we want to map the type to a given exception or just raise it as a base exception
            var longMessageProvider = ex as IFailurePointMessageProvider;
            var longMessage = longMessageProvider?.TryMakeFailurePointMessage(input);

            if (ex is CallbackFunctionException || ex is RpnEvaluationException)
            {
                // Error while trying to run a callback:
                return new FormulaEvaluationException(ex.Message, ex, longMessage);
            }
            if(ex is FormulaParserException)
            {
                // Error while trying to run a callback:
                return new FormulaParseException(ex.Message, ex, longMessage);
            }

            return new FormulaException(ex.Message, ex, longMessage);
        }
    }
}
