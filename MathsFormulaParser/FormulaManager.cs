using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Alistair.Tudor.MathsFormulaParser.Internal;
using Alistair.Tudor.MathsFormulaParser.Internal.Exceptions;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions.Impl;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser
{
    /// <summary>
    /// Main class used to create formula evaluators
    /// </summary>
    public class FormulaManager
    {
        /// <summary>
        /// Regex for flattening whitespace - compiled due to continuous use by FormulaManage
        /// </summary>
        private static readonly Regex WhitepaceFlattenRegex = new Regex(@"^\s+$", RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Dictionary of global functions. 
        /// Globally cached for all formulae as an optimisation - no point having separate delegate copies per FormulaManager instance
        /// </summary>
        private static readonly IReadOnlyDictionary<string, StandardFunction> GlobalFunctions;

        /// <summary>
        /// Dictionary of global operators. 
        /// Globally cached for all formulae as an optimisation - no point having separate delegate copies per FormulaManager instance
        /// </summary>
        private static readonly IReadOnlyList<Operator> GlobalOperators;

        /// <summary>
        /// Custom Constants dictionary
        /// </summary>
        private readonly Dictionary<string, double> _customConstantsDictionary = new Dictionary<string, double>();

        /// <summary>
        /// Dictionary of local functions
        /// </summary>
        private readonly Dictionary<string, StandardFunction> _localFunctions = new Dictionary<string, StandardFunction>();
        static FormulaManager()
        {
            // Load the default, global operators for all formulae:
            // First, get the predefined operators from BuiltInMathsSymbols:
            var operators = BuiltInMathsSymbols.GetOperatorsAndFunctions().ToArray(); // Can contain plain functions as well!

            // Add to to operator dictionary:
            GlobalOperators = operators.OfType<Operator>().ToList();

            // Get all valid System.Math functions:
            var mathFunctions = GetMathLibOperators().Distinct(new FunctionComparer());

            var allFuncs = operators.Where(o=> !(o is Operator)).Concat(mathFunctions);

            GlobalFunctions = allFuncs.Cast<StandardFunction>().ToDictionary(f=> f.FunctionName, f => f);
        }

        public FormulaManager(string inputFormula)
        {
            InputFormula = inputFormula;
            // Sanitise the input by flattening whitespace:
            InputFormula = WhitepaceFlattenRegex.Replace(InputFormula.GetStringOrDefault(""), " "); 
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
                return _localFunctions
                    .Where(kv => kv.Value is UserFunction)
                    .ToDictionary(kv => kv.Value.CastTo<UserFunction>().FriendlyName, kv => kv.Value.CallbackFunction);
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
            var userFunc = new UserFunction(name, callbackFunction, requiredArgumentsCount);
            AddCustomItemToDictionary(name, userFunc, _localFunctions);
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
            ClearCustomItemDictionary(_localFunctions);
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
            try
            {
                var lexer = new Lexer(InputFormula, GlobalOperators.Select(o => o.OperatorSymbol).Distinct());
                lexer.PerformLexicalAnalysis();
                var tokens = lexer.GetTokens();

                var mergedFunctionsList = GlobalFunctions.Values.Concat(_localFunctions.Values);

                var parser = new Parser(tokens, GlobalOperators, mergedFunctionsList, _customConstantsDictionary);

                parser.ParseTokens();
                var rpnTokens = parser.GetReversePolishNotationTokens();

                return new FormulaEvaluator(rpnTokens, InputFormula);
            }
            catch (BaseInternalFormulaException ex)
            {
                // Forward to the helper:
                throw InternalExceptionHelper.MapInternalException(ex, InputFormula);
            }
        }

        /// <summary>
        /// Extracts and creates Function wrappers for System.Math methods that are supported
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<StandardFunction> GetMathLibOperators()
        {
            return MathFunctions.GetFunctionWrappersForMath();
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
            var localName = UserFunction.VerifyUserFunctionName(name); // Check if valid
            dic.AddOrUpdateValue(localName, value);
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
