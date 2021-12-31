using System;
using System.Collections.Generic;
using System.Text;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Operators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators
{
    /// <summary>
    /// Class for converting RPN tokens to infix notation string
    /// </summary>
    internal class InfixNotationRpnEvaluator : AbstractRpnEvaluator
    {
        private readonly Stack<ParsedToken> _evalTokens = new Stack<ParsedToken>();

        public InfixNotationRpnEvaluator(ParsedToken[] tokens) : base(tokens)
        {
        }

        /// <summary>
        /// Gets the Infix form of the RPN token list
        /// </summary>
        /// <returns></returns>
        public string GetInfixForm()
        {
            this.Reset();
            while (this.HasTokens)
            {
                this.ReadNextToken();
            }
            if (_evalTokens.Count != 1)
            {
                throw new InvalidOperationException($"Expected the evaluation stack to have only 1 item, discovered { _evalTokens.Count }");
            }
            return _evalTokens.Pop().GetStringValue();
        }

        /// <summary>
        /// Tries to get the result of the evaluation. Throws an exception if not completely evaluated!
        /// </summary>
        /// <returns></returns>
        public override double GetResult()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Evaluates the given Function and it's arguments
        /// </summary>
        /// <param name="function"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected override double EvaluateFunction(FormulaFunction function, double[] arguments)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Extracts the Function from the token, verifies the correct number of arguments are available on the EvaluatorStack
        /// and exports the arguments for evaluation
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
            PushOperandToken(token);
        }

        /// <summary>
        /// Callback when an Function token is read
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if the Function should be evaluated or False to skip</returns>
        protected override bool OnFunctionToken(ParsedFunctionToken token)
        {
            var func = token.Function;
            if (_evalTokens.Count < func.RequiredNumberOfArguments)
            {
                RaiseError(token, $"Not enough tokens for function call '{ token.Function.GetPrettyFunctionString() }'");
            }

            var arguments = PopOffArguments(_evalTokens, func.RequiredNumberOfArguments); // Pop off all the tokens

            string exprValue;

            // CHECK: Is it an operator and valid arg count?
            if (func is Operator && arguments.Length >= 2)
            {
                var args = new[]
                    {
                        arguments[arguments.Length - 2],
                        arguments[arguments.Length - 1]
                    };

                // CHECK: Is it the special bit index operator (!@)?
                if (func.FunctionName == SpecialConstants.GetBitOperatorSymbol)
                {
                    // Yep: so transform to x[y]:
                    exprValue = $"{GetStringValueOfToken(args[0])}[{GetStringValueOfToken(args[1])}]";
                }
                else
                {
                    // Format:
                    // x op y
                    exprValue = $"{GetStringValueOfToken(args[0])} {func.FunctionName} {GetStringValueOfToken(args[1])}";
                }
            }
            else
            {
                // Function call style:
                // op(x,y,z)
                var builder = new StringBuilder(func.FunctionName);
                builder.Append("(");
                var first = true;
                foreach (var argument in arguments)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        builder.AppendFormat(", ");
                    }
                    builder.AppendFormat(GetStringValueOfToken(argument));
                }
                builder.AppendFormat(")");
                exprValue = builder.ToString();
            }
            PushOperandToken(new InternalExpression(exprValue));
            return false; // Stop default behaviour
        }

        /// <summary>
        /// Callback when a number token is read
        /// </summary>
        /// <param name="token"></param>
        protected override void OnNumberToken(ParsedNumberToken token)
        {
            PushOperandToken(token);
        }

        /// <summary>
        /// Callback when a variable token is read
        /// </summary>
        /// <param name="token"></param>
        protected override void OnVariableToken(ParsedVariableToken token)
        {
            PushOperandToken(token);
        }

        /// <summary>
        /// Resolves a variable name to its value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override double ResolveVariable(string name)
        {
            throw new System.NotImplementedException();
        }

        private string GetStringValueOfToken(ParsedToken t)
        {
            var constantToken = t as ParsedConstantToken;
            if (constantToken != null)
            {
                return constantToken.Name; // Return the constant name
            }
            var val = t.GetStringValue();
            return t is InternalExpression ? $"({val})" : val;
        }

        private void PushOperandToken(ParsedToken t)
        {
            _evalTokens.Push(t);
        }

        /// <summary>
        /// Represents an inner expression
        /// </summary>
        private class InternalExpression : ParsedToken
        {
            /// <summary>
            /// Value of the expression
            /// </summary>
            public string ExpressionValue { get; }

            /// <summary>
            /// Gets the value of the token as a string
            /// </summary>
            /// <returns></returns>
            public override string GetStringValue()
            {
                return ExpressionValue;
            }

            /// <summary>
            /// Gets a string description of the token
            /// </summary>
            /// <returns></returns>
            protected override string ProvideValueString()
            {
                return GetStringValue();
            }

            public InternalExpression(string expressionValue) : base(-1)
            {
                ExpressionValue = expressionValue;
            }
        }
    }
}
