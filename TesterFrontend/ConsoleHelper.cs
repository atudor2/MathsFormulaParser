using System;
using System.Collections.Generic;

namespace TesterFrontend
{
    // lifted from other project to allow compilation
    /// <summary>
    /// Utility class for Console related methods
    /// </summary>
    internal static class ConsoleHelper
    {
        /// <summary>
        /// Prompts the user for an answer to a given question
        /// </summary>
        /// <param name="question">Question to display</param>
        /// <param name="verifierCallback">Predicate used to verify the user input. Must return True to allow the answer to be accepted</param>
        /// <param name="returnCallback">Predicate used to convert the user input to True or False</param>
        /// <returns>True if the returnCallback returned True</returns>
        public static bool AskConsoleQuestionCustomVerifier(string question, Predicate<string> verifierCallback, Predicate<string> returnCallback)
        {
            var answer = AskConsoleQuestion(question, verifierCallback);
            return returnCallback(answer);
        }

        /// <summary>
        /// Prompts the user for an answer to a given question
        /// </summary>
        /// <param name="question">Question to display</param>
        /// <param name="verifierCallback">Predicate used to verify the user input. Must return True to allow the answer to be accepted</param>
        /// <returns>The string the user entered</returns>
        public static string AskConsoleQuestion(string question, Predicate<string> verifierCallback)
        {
            var answer = "";
            do
            {
                Console.Write(question);
                answer = Console.ReadLine();
            }
            while (!verifierCallback(answer));
            return answer;
        }

        /// <summary>
        /// Prompts the user for an answer to a yes or no question.
        /// </summary>
        /// <param name="question">Question to display</param>
        /// <param name="doNotShowYN">Should the string ' [Y/N] ' be appended to the end of the question string?</param>
        /// <returns>True if the user entered 'Y'</returns>
        public static bool AskConsoleQuestion(string question, bool doNotShowYN = false)
        {
            return AskConsoleQuestionCustomVerifier(
                string.Format("{0}{1}", question, doNotShowYN ? "" : " [Y/N] "),
                delegate (string responseStr)
                {
                    var answer = '\0';
                    var response = '\0';
                    if (!string.IsNullOrEmpty(responseStr) && responseStr.Length >= 1)
                    {
                        response = responseStr[0];
                    }
                    if (Char.IsLetter(response))
                    {
                        answer = Char.ToLower(response);
                    }
                    return (answer == 'y' || answer == 'n');
                },
                s => Char.ToLower(s[0]) == 'y');
        }
    }
}
