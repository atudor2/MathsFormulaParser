using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Alistair.Tudor.MathsFormulaParser.Internal;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions.Impl;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Functions;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Impl;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Operators;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser
{
    /// <summary>
    /// Main class used to create formula evaluators
    /// </summary>
    public class FormulaManager
    {
        /// <summary>
        /// Dictionary of global constants. 
        /// Globally cached for all constants as an optimisation - no point having separate copies per FormulaManager instance
        /// </summary>
        private static readonly IReadOnlyList<Constant> GlobalConstants = new Constant[]
        {
            new Constant("PI", Math.PI),
            new Constant("EU", Math.E),
        };

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
        /// Regex for flattening whitespace - compiled due to continuous use by FormulaManage
        /// </summary>
        private static readonly Regex WhitepaceFlattenRegex = new Regex(@"^\s+$", RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// Custom Constants dictionary
        /// </summary>
        private readonly Dictionary<string, Constant> _customConstantsDictionary = new Dictionary<string, Constant>();

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
            var mathFunctions = MapIdenticalMathFuncs(GetMathLibOperators()).Distinct(new FunctionComparer());//GetMathLibOperators().Distinct(new FunctionComparer());

            var allFuncs = operators.Where(o => !(o is Operator)).Concat(mathFunctions);

            GlobalFunctions = allFuncs.Cast<StandardFunction>().ToDictionary(f => f.FunctionName, f => f);
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
        /// Gets a enumerable list of built-in global functions available
        /// </summary>
        public IEnumerable<string> AvailableGlobalFunctions => GlobalFunctions.Keys;

        /// <summary>
        /// Gets a enumerable list of built-in global operators available
        /// </summary>
        public IEnumerable<string> AvailableGlobalOperators => GlobalOperators.Select(o => o.OperatorSymbol);

        /// <summary>
        /// Gets the list of currently registered callback functions
        /// </summary>
        public IReadOnlyDictionary<string, FormulaCallbackFunction> CustomCallbackFunctions
        {
            get
            {
                return _localFunctions
                    .Select(kv => kv.Value)
                    .OfType<UserFunction>()
                    .ToDictionary(kv => kv.FriendlyName, kv => kv.CallbackFunction);
            }
        }

        /// <summary>
        /// Gets the list of currently registered constants
        /// </summary>
        public IReadOnlyDictionary<string, double> CustomConstants => _customConstantsDictionary.ToDictionary(k => k.Key, v => v.Value.Value);

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
            AddCustomItemToDictionary(name, new Constant(name, value), _customConstantsDictionary);
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
            var lexer = new Lexer(InputFormula, GlobalOperators.Select(o => o.OperatorSymbol).Distinct());
            lexer.PerformLexicalAnalysis();
            var tokens = lexer.GetTokens();

            var mergedFunctionsList = GlobalFunctions.Values.Concat(_localFunctions.Values);
            var mergedConstantsList = GlobalConstants.Concat(_customConstantsDictionary.Values);

            var parser = new Parser(InputFormula, tokens, GlobalOperators, mergedFunctionsList, mergedConstantsList);

            parser.ParseTokens();

            var rpnTokens = parser.GetReversePolishNotationTokens();
            return new FormulaEvaluator(rpnTokens, InputFormula);
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
        /// Maps functions to different names if needed to cope with differing arity functions
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static IEnumerable<StandardFunction> MapIdenticalMathFuncs(IEnumerable<StandardFunction> input)
        {
            foreach (var function in input)
            {
                // Map known Math lib functions to different names:
                switch (function.FunctionName.ToLower())
                {
                    case "log":
                        // Only 2 variants:
                        // 1 arg => Base E
                        if (function.RequiredNumberOfArguments == 1)
                        {
                            yield return (StandardFunction)function.RenameFunction("ln");
                            continue;
                        }
                        break;
                }
                yield return function;
            }
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
    }
}
