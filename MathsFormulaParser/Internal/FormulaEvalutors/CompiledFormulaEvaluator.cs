using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors
{
    /// <summary>
    /// Represents a formula evaluator where formula execution is compiled
    /// </summary>
    internal class CompiledFormulaEvaluator : IInternalFormulaEvaluator
    {
        /// <summary>
        /// Compiled expression lambda
        /// </summary>
        private readonly Execute _compiledLambda;

        public CompiledFormulaEvaluator(ParsedToken[] rpnTokens)
        {
            RpnTokens = rpnTokens;
            _compiledLambda = CompileExpression(rpnTokens);
        }

        /// <summary>
        /// Delegate for compiled expression
        /// </summary>
        /// <param name="variableMap"></param>
        /// <returns></returns>
        private delegate double Execute(IDictionary<string, double> variableMap);
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
            return _compiledLambda(variableMap);
        }

        /// <summary>
        /// Compiles the given set of tokens
        /// </summary>
        /// <param name="rpnTokens"></param>
        private Execute CompileExpression(ParsedToken[] rpnTokens)
        {
            if (rpnTokens.Length == 1)
            {
                var token = rpnTokens[0];
                if (token is ParsedValueToken)
                {
                    return (i) => ((ParsedValueToken) token).Value;
                }
                if (!(token is ParsedVariableToken) && !(token is ParsedValueToken))
                {
                    RaiseError($"Unexpected token type '{token}'");
                }
            }

            var tokens = rpnTokens.Reverse();
            // ((A + B - (33 + 3)) * 2)
            // A B + 33 3 + - 2 *
            // * 2 - + 3 33 + A B
            foreach (var token in tokens)
            {
                if (!(token is ParsedFunctionToken))
                {
                    // ERROR!
                    RaiseError("Expected an operator as the first token");
                }
            }

            throw new NotImplementedException();
        }

        private void RaiseError(string msg)
        {
            throw new NotImplementedException();
        }
    }
}
