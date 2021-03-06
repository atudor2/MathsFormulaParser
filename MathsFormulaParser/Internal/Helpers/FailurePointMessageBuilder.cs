using System.Text;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    /// <summary>
    /// Helper class to make a failure point message
    /// <returns>NULL if it's not possible to build a message</returns>
    /// </summary>
    internal static class FailurePointMessageBuilder
    {
        public static string? MakeMessage(string input, string errMsg, long failurePoint, string additionalInfo = "")
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

             [Additional Info if provided]
            */

            var builder = new StringBuilder(errMsg).AppendLine().AppendLine().AppendLine(input);
            // Offset by 1 to convert to 0 index:
            failurePoint--;
            failurePoint = SanitiseFailurePoint(input, failurePoint);

            if (failurePoint > 0)
            {
                builder.Append(new string('-', (int) failurePoint));
            }

            builder.Append('^');

            if (!string.IsNullOrWhiteSpace(additionalInfo))
            {
                builder.AppendLine().AppendLine().Append(additionalInfo);
            }

            return builder.ToString();
        }

        private static long SanitiseFailurePoint(string input, long failurePoint)
        {
            if (failurePoint < 0) failurePoint = 0;
            if (failurePoint >= input.Length) failurePoint = input.Length - 1;
            return failurePoint;
        }
    }
}
