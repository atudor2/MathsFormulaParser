﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;

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
        /// Should extended checks be run when executing operators?
        /// </summary>
        public override bool PerformExtendedChecks
        {
            get { return false; } // Never
            set { /* NOP */ }
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

        public override double GetResult()
        {
            throw new System.NotImplementedException();
        }

        protected override double EvaluateFunction(StandardFunction function, double[] arguments)
        {
            throw new System.NotImplementedException();
        }

        protected override void ExtractAndVerifyFunctionInfo(ParsedFunctionToken token, out StandardFunction function, out double[] arguments)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnConstantToken(ParsedConstantToken token)
        {
            PushOperandToken(token);
        }

        protected override void OnNumberToken(ParsedNumberToken token)
        {
            PushOperandToken(token);
        }

        protected override bool OnFunctionToken(ParsedFunctionToken token)
        {
            var func = token.Function;
            var arguments = _evalTokens.PopOff(func.RequiredNumberOfArguments).Reverse().ToArray(); // Pop off all the tokens
                                                                                                  
            string exprValue;

            // CHECK: Is it an operator and valid arg count?

            if (func is Operator && arguments.Length >= 2)
            {
                // Format:
                // x op y
                var args = new[]
                {
                        arguments[arguments.Length - 2],
                        arguments[arguments.Length - 1]
                    };
                exprValue = $"{GetStringValueOfToken(args[0])} {func.FunctionName} {GetStringValueOfToken(args[1])}";
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
            PushOperandToken(new InternalExpression() { ExpressionValue = exprValue });
            return false; // Stop default behaviour
        }

        protected override void OnVariableToken(ParsedVariableToken token)
        {
            PushOperandToken(token);
        }

        protected override double ResolveVariable(string name)
        {
            throw new System.NotImplementedException();
        }

        private string GetStringValueOfToken(ParsedToken t)
        {
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
            public string ExpressionValue { get; set; }
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
        }
    }
}
