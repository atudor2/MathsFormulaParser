using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators.Impl;
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
        /// <summary>
        /// Precedence for System.Math functions
        /// </summary>
        private const int MathFuncPrecedence = OperatorConstants.FunctionPrecedence;

        /// <summary>
        /// Custom Item name validation regex
        /// </summary>
        private static readonly Regex CustomNameCheckRegex = new Regex(@"^[A-Z0-9_]+$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /// <summary>
        /// Dictionary of global operators. 
        /// Globally cached for all formulae as an optimisation - no point having separate delegate copies per FormulaManager instance
        /// </summary>
        private static readonly IReadOnlyDictionary<string, Operator> GlobalOperators;

        /// <summary>
        /// Custom callback functions dictionary
        /// </summary>
        private readonly Dictionary<string, CallbackFunctionHolder> _customCallbackFunctions = new Dictionary<string, CallbackFunctionHolder>();

        /// <summary>
        /// Custom Constants dictionary
        /// </summary>
        private readonly Dictionary<string, double> _customConstantsDictionary = new Dictionary<string, double>();

        static FormulaManager()
        {
            // Load the default, global operators for all formulae:
            // First, get the predefined operators from MathsOperators:
            var operators = MathsOperators.GetOperators();
            // Get all valid System.Math functions:
            var mathFunctions = GetMathLibOperators().Distinct(new OperatorComparer());

            var allOperators = operators.Concat(mathFunctions);

            GlobalOperators = allOperators.ToDictionary(o => o.OperatorSymbol, o => o);
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

            // Make a dictionary of the callback functions as operators:
            var callbackFuncOperatorDic = _customCallbackFunctions.ToDictionary(c => c.Key, c => MakeCallbackFunctionOperator(c.Key, c.Value));
            // Create a merged dictionary of the global operators and custom functions:
            var mergedDic = GlobalOperators.Concat(callbackFuncOperatorDic).ToDictionary(x => x.Key, x => x.Value);

            var parser = new Parser(tokens, mergedDic, _customConstantsDictionary);

            parser.ParseTokens();
            var rpnTokens = parser.GetReversePolishNotationTokens();

            return new FormulaEvaluator(rpnTokens, InputFormula);
        }

        /// <summary>
        /// Extracts and creates Operator wrappers for System.Math methods that are supported
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Operator> GetMathLibOperators()
        {
            var mathType = typeof(Math);
            var methods = mathType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods.Where(CheckMathLibMethod))
            {
                var @params = method.GetParameters().Length;

                // Make an operator:
                // Need to wrap it to Invoke() the MethodInfo
                FormulaCallbackFunction thunk = (i) => (double)method.Invoke(null, i.Select(x => (object)x).ToArray());

                var evaluateFunc = thunk;

                var name = method.Name.ToLower();

                var op = new GenericOperator(MathFuncPrecedence, name, OperatorAssociativity.Left, @params, evaluateFunc);
                yield return op;
            }
        }

        private static bool CheckMathLibMethod(MethodInfo method)
        {
            var validReturnType = method.ReturnType == typeof(double);
            var validParams = method.GetParameters().All(p => p.ParameterType == typeof(double));

            return validParams && validReturnType;
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

        /// <summary>
        /// Creates an Operator wrapper for a callback function
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callbackFunctionHolder"></param>
        /// <returns></returns>
        private Operator MakeCallbackFunctionOperator(string name, CallbackFunctionHolder callbackFunctionHolder)
        {
            return new GenericOperator(MathFuncPrecedence, name, OperatorAssociativity.Left, callbackFunctionHolder.ArgumentCount, callbackFunctionHolder.Callback);
        }

        /// <summary>
        /// Internal class for holding callback function
        /// </summary>
        private class CallbackFunctionHolder
        {
            public int ArgumentCount { get; set; }
            public FormulaCallbackFunction Callback { get; set; } = null;
        }
    }
}
