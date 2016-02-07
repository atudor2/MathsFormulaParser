using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    /// <summary>
    /// Helper class to make a failure point message
    /// <returns>NULL if it's not possible to build a message</returns>
    /// </summary>
    internal static class FailurePointMessageBuilder
    {
        public static string MakeMessage(string input, string errMsg, long failurePoint)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(errMsg) || failurePoint < 0)
            {
                // Nothing to do:
                return null;
            }

            /*
             FORMAT:
             [Error message]

             [BLAH BLAH BLAH]
             ---------^
            */

            var builder = new StringBuilder(errMsg).AppendLine().AppendLine().AppendLine(input);
            // Offset by 1 to convert to 0 index:
            failurePoint--;
            failurePoint = SanitiseFailurPoint(input, failurePoint);

            if (failurePoint > 0)
            {
                builder.Append(new string('-', (int) failurePoint));
            }

            builder.Append("^");

            return builder.ToString();
        }

        private static long SanitiseFailurPoint(string input, long failurePoint)
        {
            if (failurePoint < 0) failurePoint = 0;
            if (failurePoint >= input.Length) failurePoint = input.Length - 1;
            return failurePoint;
        }
    }
}
