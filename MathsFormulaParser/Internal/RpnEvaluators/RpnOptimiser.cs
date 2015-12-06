using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;

namespace Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators
{
    /// <summary>
    /// Special RpnEvaluator that optimises the expression
    /// </summary>
    internal class RpnOptimiser : AbstractRpnEvaluator
    {
        /// <summary>
        /// Output token stack
        /// </summary>
        private readonly Stack<ParsedToken> _tokenStack = new Stack<ParsedToken>();

        public RpnOptimiser(ParsedToken[] tokens) : base(tokens)
        {
        }

        /// <summary>
        /// Tries to get the result of the evaluation. Throws an exception if not completely evaluated!
        /// NOT A VALID CALL FOR THIS CLASS!
        /// </summary>
        /// <returns></returns>
        public override double GetResult()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Optimises the input list of parsed tokens, returning the optimised list of tokens.
        /// </summary>
        /// <returns></returns>
        public ParsedToken[] OptimiseExpression()
        {
            while (this.HasTokens)
            {
                this.ReadNextToken();
            }
            var rawTokenStackItems = _tokenStack.ToArray();
            var lst = new List<ParsedToken>();
            lst.AddRange(FlattenRawTokens(rawTokenStackItems));
            return lst.ToArray();
        }

        /// <summary>
        /// Extracts the Function from the token, verifies the correct number of arguments are available on the EvaluatorStack
        /// and exports the arguments for evaluation
        /// NOT A VALID CALL FOR THIS CLASS!
        /// </summary>
        /// <param name="token"></param>
        /// <param name="function"></param>
        /// <param name="arguments"></param>
        protected override void ExtractAndVerifyFunctionInfo(ParsedFunctionToken token, out FormulaFunction function, out double[] arguments)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Callback when a constant token is read
        /// </summary>
        /// <param name="token"></param>
        protected override void OnConstantToken(ParsedConstantToken token)
        {
            PushConstantValue(token.Value);
        }

        /// <summary>
        /// Callback when a number token is read
        /// </summary>
        /// <param name="token"></param>
        protected override void OnNumberToken(ParsedNumberToken token)
        {
            _tokenStack.Push(token); // Simply push it to the stack
        }

        /// <summary>
        /// Callback when an Function token is read
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if the Function should be evaluated or False to skip</returns>
        protected override bool OnFunctionToken(ParsedFunctionToken token)
        {
            var @operator = token.Function;
            var argCount = @operator.RequiredNumberOfArguments;
            if (argCount > _tokenStack.Count)
            {
                // Not enough args!
                RaiseError(token, $"Not enough arguments for '{token.Value}': Expected '{argCount}', but only '{EvaluatorStack.Count}' are available");
            }
            // Pop off the args:
            var arguments = _tokenStack.PopOff(argCount).Reverse().ToArray();
            if (arguments.Any(a => !(a is ParsedNumberToken)))
            {
                // Argument has a variable - cannot flatten:
                // Work around this by creating a fake variable token holding the tokens which will be added again to the 
                // token list at the end
                var expressionTokens = new ParsedToken[argCount + 1];
                arguments.CopyTo(expressionTokens, 0);
                expressionTokens[argCount] = token; // Place token at the end

                // Push this fake variable to the stack:
                _tokenStack.Push(new TempExpressionVariableParsedToken(expressionTokens));

                goto EXIT;
            }
            var result = EvaluateFunction(@operator, arguments.Cast<ParsedNumberToken>().Select(a => a.Value).ToArray());
            PushConstantValue(result);

            EXIT:
            return false; // Suppress default Function handling...
        }

        /// <summary>
        /// Callback when a variable token is read
        /// </summary>
        /// <param name="token"></param>
        protected override void OnVariableToken(ParsedVariableToken token)
        {
            _tokenStack.Push(token); // Simply push it to the stack
        }

        /// <summary>
        /// Resolves a variable name to its value
        ///  NOT A VALID CALL FOR THIS CLASS!
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override double ResolveVariable(string name)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Flattens the 'TempExpressionVariableParsedToken' tokens in the input list of tokens 
        /// </summary>
        /// <param name="rawTokenStackItems"></param>
        /// <returns></returns>
        private IEnumerable<ParsedToken> FlattenRawTokens(ParsedToken[] rawTokenStackItems)
        {
            // TODO: Recursive YIELD RETURN! Very bad for memory usage! Find another way!
            foreach (var token in rawTokenStackItems)
            {
                var t = token as TempExpressionVariableParsedToken;
                if (t == null)
                {
                    // Normal token, return:
                    yield return token;
                    continue;
                }
                // Flatten it:
                foreach (var rawToken in FlattenRawTokens(t.ExpressionTokens))
                {
                    yield return rawToken;
                }
            }
        }

        /// <summary>
        /// Pushes a constant value to the output token stack
        /// </summary>
        /// <param name="value"></param>
        private void PushConstantValue(double value)
        {
            _tokenStack.Push(new ParsedNumberToken(value));
        }

        /// <summary>
        /// Helper class used to hold expressions that cannot be flattened
        /// </summary>
        private class TempExpressionVariableParsedToken : ParsedVariableToken
        {
            /// <summary>
            /// Random generator for the name
            /// </summary>
            private static readonly Random RandomSource = new Random();

            public TempExpressionVariableParsedToken(ParsedToken[] expressionTokens) : base($"$TMP_EXPR_{RandomSource.Next(0, 999)}$")
            {
                ExpressionTokens = expressionTokens;
            }

            /// <summary>
            /// Gets the tokens of this temp. variable
            /// </summary>
            public ParsedToken[] ExpressionTokens { get; }
        }
    }
}
