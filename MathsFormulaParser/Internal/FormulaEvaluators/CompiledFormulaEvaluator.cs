using System.Collections.Generic;
using System.Diagnostics;
using Alistair.Tudor.MathsFormulaParser.Exceptions;
using Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvaluators.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;

namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvaluators
{
    /// <summary>
    /// Represents a formula evaluator where formula execution is compiled
    /// </summary>
    internal class CompiledFormulaEvaluator : IInternalFormulaEvaluator, IVariableResolver
    {
        /// <summary>
        /// Compiled expression lambda
        /// </summary>
        private readonly CompiledFormulaExpression _compiledLambda;

        /// <summary>
        /// Current variable map
        /// </summary>
        private IDictionary<string, double>? _currentVariableMap;

        /// <summary>
        /// Internal field only FILLED in DEBUG
        /// </summary>
        private string? _lambdaDebugView;

        public CompiledFormulaEvaluator(ParsedToken[] rpnTokens)
        {
            RpnTokens = rpnTokens;
            _compiledLambda = CompileExpression(rpnTokens);
        }

        /// <summary>
        /// Current RPN tokens
        /// </summary>
        public ParsedToken[] RpnTokens { get; }

        /// <summary>
        /// Debug view of Lambda - may be NULL if in release mode or not available
        /// </summary>
        public string? LambdaDebugView => _lambdaDebugView;

        /// <summary>
        /// Evaluates the formula with the given variables
        /// </summary>
        /// <param name="variableMap"></param>
        /// <returns></returns>
        public double Evaluate(IDictionary<string, double> variableMap)
        {
            try
            {
                _currentVariableMap = variableMap;
                return _compiledLambda();
            }
            finally
            {
                _currentVariableMap = null;
            }
        }

        /// <summary>
        /// Resolves a given variable. Throws an exception if variable is not found
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public double ResolveVariable(string name)
        {
            Debug.Assert(_currentVariableMap != null, $"{nameof(_currentVariableMap)} != null");

            if (!_currentVariableMap.TryGetValue(name.ToUpper(), out var varValue))
            {
                throw CreateError($"Cannot find variable '{ name }''");
            }
            return varValue;
        }

        /// <summary>
        /// Creates a FormulaEvaluationException
        /// </summary>
        /// <param name="msg">Main error message</param>
        /// <param name="additionalInfo">Any optional/additional information relating to the main error</param>
        private static FormulaEvaluationException CreateError(string msg, string additionalInfo = "")
        {
            return new FormulaEvaluationException(msg, additionalInfo: additionalInfo);
        }

        /// <summary>
        /// Compiles the given set of tokens
        /// </summary>
        /// <param name="rpnTokens"></param>
        private CompiledFormulaExpression CompileExpression(ParsedToken[] rpnTokens)
        {
            // If only 1 token, then we can just return/resolve it directly 
            // instead of making an Expression tree
            if (rpnTokens.Length == 1)
            {
                var token = rpnTokens[0];
                return token switch
                {
                    ParsedValueToken valueToken => () => valueToken.Value,
                    ParsedVariableToken varToken => () => ResolveVariable(varToken.Name),
                    _ => throw CreateError($"Unexpected token type '{token}'")
                };
            }

            // We are going to do this like a normal RPN evaluation,
            // Except that it will store Expressions
            var compiler = new RpnCompiler(rpnTokens, this);
            return compiler.CompileExpression(out _lambdaDebugView);
        }
    }
}
