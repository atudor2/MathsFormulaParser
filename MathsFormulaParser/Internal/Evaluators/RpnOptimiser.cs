using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Evaluators
{
    /// <summary>
    /// Special RpnEvaluator that optimises the expression
    /// </summary>
    internal class RpnOptimiser : AbstractRpnEvaluator
    {
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
            InvalidMethodCall();
            return double.MinValue;
        }


        public ParsedToken[] OptimiseExpression()
        {
            while (this.HasTokens)
            {
                this.ReadNextToken();
            }
            var rawTokenStackItems =  _tokenStack.ToArray();
            var lst = new List<ParsedToken>();
            lst.AddRange(FlattenRawTokens(rawTokenStackItems));
            return lst.ToArray();
        }

        /// <summary>
        /// Extracts the operator from the token, verifies the correct number of arguments are available on the EvaluatorStack
        /// and exports the arguments for evaluation
        /// NOT A VALID CALL FOR THIS CLASS!
        /// </summary>
        /// <param name="token"></param>
        /// <param name="operator"></param>
        /// <param name="arguments"></param>
        protected override void ExtractAndVerifyOperatorInfo(ParsedOperatorToken token, out Operator @operator, out double[] arguments)
        {
            InvalidMethodCall();
            @operator = null;
            arguments = null;
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
        /// Callback when an operator token is read
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if the operator should be evaluated or False to skip</returns>
        protected override bool OnOperatorToken(ParsedOperatorToken token)
        {
            var t = token;
            var @operator = token.Operator;
            var argCount = @operator.RequiredNumberOfArguments;
            if (argCount > _tokenStack.Count)
            {
                // Not enough args!
                RaiseError(token, $"Not enough arguments for '{ t.Value }': Expected '{ argCount  }', but only '{ EvaluatorStack.Count }' are available");
            }
            // Pop off the args:
            var arguments = _tokenStack.PopOff(argCount).Reverse().ToArray();
            if (arguments.Any(a => !(a is ParsedNumberToken)))
            {
                // Argument has a variable - cannot flatten:
                // Work around this by creating a fake vairable token holding the tokens which will be added again to the 
                // token list at the end
                var expressionTokens = new ParsedToken[argCount + 1];
                arguments.CopyTo(expressionTokens, 0);
                expressionTokens[argCount] = t; // Place token at the end

                // Push this fake variable to the stack:
                _tokenStack.Push(new TempExpressionVariableParsedToken(expressionTokens));

                goto EXIT;
            }
            var result = EvaluateOperator(@operator, arguments.Cast<ParsedNumberToken>().Select(a => a.Value).ToArray());
            PushConstantValue(result);

            EXIT:
            return false; // Supress default operator handling...
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
            InvalidMethodCall();
            return double.MinValue;
        }

        /// <summary>
        /// Simple helper method for raising exception on invalid method call
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="caller"></param>
        private static void InvalidMethodCall(string msg = null, [CallerMemberName] string caller = null)
        {
            caller = !string.IsNullOrWhiteSpace(caller) ? $"{caller}()" : "[Unknown]";
            var baseMsg = $"The call to '{caller}' is not valid for the Optimiser";
            if (!string.IsNullOrWhiteSpace(msg))
            {
                baseMsg = $"{baseMsg}: {msg}";
            }
            throw new InvalidOperationException(baseMsg);
        }

        /// <summary>
        /// Flattens the 'TempExpressionVariableParsedToken' tokens in the input list of tokens 
        /// </summary>
        /// <param name="rawTokenStackItems"></param>
        /// <returns></returns>
        private IEnumerable<ParsedToken> FlattenRawTokens(ParsedToken[] rawTokenStackItems)
        {
            // TODO: Recursive YIELD RETURN! Find another way!
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
        private void PushConstantValue(double value)
        {
            _tokenStack.Push(new ParsedNumberToken(value));
        }

        /// <summary>
        /// Helper class used to hold expressions that cannot be flattened
        /// </summary>
        private class TempExpressionVariableParsedToken : ParsedVariableToken
        {
            private static readonly Random RandomSource = new Random();
            public TempExpressionVariableParsedToken(ParsedToken[] expressionTokens) : base($"$TMP_EXPR_{RandomSource.Next(0, 999)}$")
            {
                ExpressionTokens = expressionTokens;
            }

            public ParsedToken[] ExpressionTokens { get; }
        }
    }
}
