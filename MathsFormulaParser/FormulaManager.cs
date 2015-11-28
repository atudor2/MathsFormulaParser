using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.LexicalAnalysis;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser
{
    /// <summary>
    /// Main class used to create formula evaluators
    /// </summary>
    public class FormulaManager
    {
        private static readonly Regex CustomNameCheckRegex = new Regex(@"^[A-Z0-9_]+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private readonly Dictionary<string, CallbackFunctionHolder> _customCallbackFunctions = new Dictionary<string, CallbackFunctionHolder>();
        private readonly Dictionary<string, double> _customConstantsDictionary = new Dictionary<string, double>();

        private class CallbackFunctionHolder
        {
            public FormulaCallbackFunction Callback { get; set; } = null;
            public int ArgumentCount { get; set; }
        }

        public FormulaManager(string inputFormula)
        {
            InputFormula = inputFormula;
            if (string.IsNullOrWhiteSpace(inputFormula))
            {
                throw new ArgumentException(nameof(inputFormula));
            }
        }

        /// <summary>
        /// Gets the list of currently registered callback functions
        /// </summary>
        public IReadOnlyDictionary<string, FormulaCallbackFunction> CustomCallbackFunctions
        {
            get
            {
                return GetFriendlyCustomItemDictionary(_customCallbackFunctions)
                                .Select(i => new KeyValuePair<string, FormulaCallbackFunction>(i.Key, i.Value.Callback))
                                .ToDictionary();
            }
        }

        /// <summary>
        /// Gets the list of currently registered constants
        /// </summary>
        public IReadOnlyDictionary<string, double> CustomConstants => GetFriendlyCustomItemDictionary(_customConstantsDictionary).ToDictionary();

        /// <summary>
        /// Gets the original input formula
        /// </summary>
        public string InputFormula { get; set; }

        /// <summary>
        /// Adds or updates a callback function
        /// </summary>
        /// <param name="name">Name of the call back</param>
        /// <param name="callbackFunction"></param>
        /// <param name="requiredArgumentsCount"></param>
        /// <remarks>All custom functions will have a '_' prepended to the name by the register function. DO NOT PASS A NAME STARTING WITH '_'</remarks>
        public void AddCustomCallbackFunction(string name, FormulaCallbackFunction callbackFunction, int requiredArgumentsCount)
        {
            var holder = new CallbackFunctionHolder()
            {
                Callback = callbackFunction,
                ArgumentCount = requiredArgumentsCount
            };
            AddCustomItemToDictionary(name, holder, _customCallbackFunctions);
        }

        /// <summary>
        /// Adds or updates a constant
        /// </summary>
        /// <param name="name">Name of the constant</param>
        /// <param name="value"></param>
        /// <remarks>All constant will have a '_' prepended to the name by the register function. DO NOT PASS A NAME STARTING WITH '_'</remarks>
        public void AddCustomConstant(string name, double value)
        {
            AddCustomItemToDictionary(name, value, _customConstantsDictionary);
        }

        /// <summary>
        /// Clears all custom callback functions
        /// </summary>
        public void ClearCustomCallbackFunctions()
        {
            ClearCustomItemDictionary(_customCallbackFunctions);
        }

        /// <summary>
        /// Clears all custom constants
        /// </summary>
        public void ClearCustomConstants()
        {
            ClearCustomItemDictionary(_customConstantsDictionary);
        }

        /// <summary>
        /// Creates a formula evaluator with all the custom callbacks, constants etc
        /// </summary>
        /// <returns></returns>
        public IFormulaEvaluator CreateFormulaEvaluator()
        {
            LexicalToken[] tokens;
            using (var reader = new StringReader(InputFormula))
            {
                var lexer = new Lexer(reader);
                lexer.PerformLexicalAnalysis();
                tokens = lexer.GetTokens();
            }
            var parser = new Parser(tokens);

            // Add the custom constants:
            foreach (var constant in _customConstantsDictionary)
            {
                parser.RegisterCustomConstant(constant.Key, constant.Value);
            }

            // Add the custom functions:
            foreach (var callbackFunction in _customCallbackFunctions)
            {
                parser.RegisterCustomFunction(callbackFunction.Key, callbackFunction.Value.Callback, callbackFunction.Value.ArgumentCount);
            }

            parser.ParseTokens();
            var rpnTokens = parser.GetReversePolishNotationTokens();

            return new FormulaEvaluator(rpnTokens, InputFormula);
        }

        /// <summary>
        /// Helper to add a item to the dictionary after validating name
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="dic"></param>
        private void AddCustomItemToDictionary<TValue>(string name, TValue value, Dictionary<string, TValue> dic)
        {
            var localName = CheckCustomCallbackName(name); // Check if valid
            dic.AddOrUpdateValue(localName, value);
        }

        /// <summary>
        /// Checks the custom item name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string CheckCustomCallbackName(string name)
        {
            // Check the name:
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException($"Name cannot be null or whitespace!");
            var localName = name;
            if (name.StartsWith("_"))
            {
                if (name.Length <= 1)
                {
                    // Not valid! Only '_'
                    throw new ArgumentException($"Name cannot only be a '_' character!");
                }
                localName = name.Substring(1);
            }

            // Check valid:
            if (!CustomNameCheckRegex.IsMatch(localName))
            {
                throw new ArgumentException("Name must only contain A-Z, 0-9 and _");
            }

            return "_" + localName.ToLower();
        }

        /// <summary>
        /// Helper to clear dictionary
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        private void ClearCustomItemDictionary<TValue>(IDictionary<string, TValue> dic)
        {
            dic.Clear();
        }

        /// <summary>
        /// Makes a 'friendly' custom item dictionary by removing '_' from start
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, TValue>> GetFriendlyCustomItemDictionary<TValue>(IDictionary<string, TValue> dic)
        {
            return dic.Select(kv => new KeyValuePair<string, TValue>(kv.Key, kv.Value));
        }
    }
}
