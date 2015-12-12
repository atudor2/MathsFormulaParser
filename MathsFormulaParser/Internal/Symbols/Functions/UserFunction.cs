using System;
using System.Text.RegularExpressions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Functions
{
    /// <summary>
    /// Represents a User Function. It is not defined by the library and is registered with a leading '_'
    /// </summary>
    internal class UserFunction : StandardFunction
    {
        /// <summary>
        /// Custom Item functionName validation regex
        /// </summary>
        private static readonly Regex CustomNameCheckRegex = new Regex(@"^[A-Z0-9_]+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public UserFunction(string functionName, FormulaCallbackFunction callbackFunction, int requiredNumberOfArguments) : 
            base(VerifyUserFunctionName(functionName), callbackFunction, requiredNumberOfArguments)
        {
            FriendlyName = functionName;
        }

        /// <summary>
        /// 'Friendly' name of the function (i.e. original name without preceding '_')
        /// </summary>
        public string FriendlyName { get; }

        /// <summary>
        /// Renames the current function
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public override FormulaFunction RenameFunction(string newName)
        {
            var userNewName = VerifyUserFunctionName(newName);
            return base.RenameFunction(userNewName);
        }
        /// <summary>
        /// Verifies the given user function name is valid
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        internal static string VerifyUserFunctionName(string functionName)
        {
            // Check the name:
            if (string.IsNullOrWhiteSpace(functionName)) throw new ArgumentException($"Name cannot be null or whitespace!");
            var localName = functionName;
            if (functionName.StartsWith("_"))
            {
                if (functionName.Length <= 1)
                {
                    // Not valid! Only '_'
                    throw new ArgumentException($"Name cannot only be a '_' character!");
                }
                localName = functionName.Substring(1);
            }

            // Check valid:
            if (!CustomNameCheckRegex.IsMatch(localName))
            {
                throw new ArgumentException("Name must only contain A-Z, 0-9 and _");
            }

            return "_" + localName.ToLower();
        }
    }
}
