using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;

namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors
{
    /// <summary>
    /// Represents a formula evaluator where formula execution is compiled
    /// </summary>
    internal class CompiledFormulaEvaluator : IInternalFormulaEvaluator, IIVariableResolver
    {
        /// <summary>
        /// Compiled expression lambda
        /// </summary>
        private readonly CompiledFormulaExpression _compiledLambda;

        /// <summary>
        /// Current variable map
        /// </summary>
        private IDictionary<string, double> _currentVariableMap;

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
            double varValue;
            if (!_currentVariableMap.TryGetValue(name.ToUpper(), out varValue))
            {
                RaiseError($"Cannot find variable '{ name }''");
            }
            return varValue;
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
                if (token is ParsedValueToken)
                {
                    return () => ((ParsedValueToken) token).Value;
                }
                if (token is ParsedVariableToken)
                {
                    var varToken = (ParsedVariableToken) token;
                    return () => ResolveVariable(varToken.Name);
                }
                RaiseError($"Unexpected token type '{token}'");
            }

            // We are going to do this like a normal RPN evaluation,
            // Except that it will store Expressions
            var compiler = new RpnCompiler(rpnTokens, this);
            return compiler.CompileExpression();
        }
        private void RaiseError(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
