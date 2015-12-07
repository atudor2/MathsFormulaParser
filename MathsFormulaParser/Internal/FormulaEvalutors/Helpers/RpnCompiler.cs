using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions;
using Alistair.Tudor.MathsFormulaParser.Internal.Functions.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.Helpers
{
    /// <summary>
    /// Class for converting RPN tokens to infix notation string
    /// </summary>
    internal class RpnCompiler : AbstractRpnEvaluator
    {
        private readonly Stack<ParsedToken> _evalTokens = new Stack<ParsedToken>();

        public RpnCompiler(ParsedToken[] tokens) : base(tokens)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CompileExpression()
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

        protected override double EvaluateFunction(FormulaFunction function, double[] arguments)
        {
            throw new System.NotImplementedException();
        }

        protected override void ExtractAndVerifyFunctionInfo(ParsedFunctionToken token, out FormulaFunction function, out double[] arguments)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnConstantToken(ParsedConstantToken token)
        {
            PushOperandToken(token);
        }

        protected override bool OnFunctionToken(ParsedFunctionToken token)
        {
            var func = token.Function;
            if (_evalTokens.Count < func.RequiredNumberOfArguments)
            {
                RaiseError(token, $"Not enough tokens for function call '{ token.Function.GetPrettyFunctionString() }'");
            }

            var arguments = _evalTokens.PopOff(func.RequiredNumberOfArguments).Reverse().ToArray(); // Pop off all the tokens

            var callback = func.CallbackFunction;

            throw new NotImplementedException();
        }

        protected override void OnNumberToken(ParsedNumberToken token)
        {
            PushOperandToken(token);
        }
        protected override void OnVariableToken(ParsedVariableToken token)
        {
            PushOperandToken(token);
        }

        protected override double ResolveVariable(string name)
        {
            throw new System.NotImplementedException();
        }

        private void PushOperandToken(ParsedToken t)
        {
            _evalTokens.Push(t);
        }
    }
}
