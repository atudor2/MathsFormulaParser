using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors.Helpers
{
    /// <summary>
    /// Class for converting RPN tokens to infix notation string
    /// </summary>
    internal class RpnCompiler : AbstractRpnEvaluator
    {
        /// <summary>
        /// Stack of expressions
        /// </summary>
        private readonly Stack<Expression> _evalTokens = new Stack<Expression>();

        /// <summary>
        /// Lazy MethodInfo for variable resolver
        /// </summary>
        private readonly Lazy<MethodInfo> _lazyVarProviderMethod = new Lazy<MethodInfo>(() => typeof(IIVariableResolver).GetMethod(nameof(IIVariableResolver.ResolveVariable)));

        /// <summary>
        /// Variable resolver reference
        /// </summary>
        private readonly IIVariableResolver _resolver;

        public RpnCompiler(ParsedToken[] tokens, IIVariableResolver resolver) : base(tokens)
        {
            resolver.ThrowIfNull(nameof(resolver));
            _resolver = resolver;
        }

        /// <summary>
        /// Compiles the given expression into a delegate
        /// </summary>
        /// <returns></returns>
        public CompiledFormulaExpression CompileExpression()
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
            var item = (MethodCallExpression) _evalTokens.Pop();

            var @delegate = Expression.Lambda<CompiledFormulaExpression>(item).Compile();
            return @delegate;
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
            PushValueOperandToken(token);
        }

        /// <summary>
        /// Callback when an Function token is read
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if the Function should be evaluated or False to skip</returns>
        protected override bool OnFunctionToken(ParsedFunctionToken token)
        {
            var func = token.Function;
            if (!func.CheckCorrectArgumentCount(_evalTokens.Count))
            {
                RaiseError(token, $"Not enough tokens for function call '{ token.Function.GetPrettyFunctionString() }'");
            }

            var arguments = PopOffArguments(_evalTokens, func.RequiredNumberOfArguments);
            var argumentInput = Expression.NewArrayInit(typeof (double), arguments);

            // Use the target for the instance:
            var instance = func.CallbackFunction.Target;
            ConstantExpression instanceExpression = null;
            if (instance != null)
            {
                instanceExpression = Expression.Constant(instance);
            }

            // Call the callback directly:
            var methodCall = Expression.Call(instanceExpression, func.CallbackFunction.Method, argumentInput);

            _evalTokens.Push(methodCall);

            return false;
        }

        /// <summary>
        /// Callback when a number token is read
        /// </summary>
        /// <param name="token"></param>
        protected override void OnNumberToken(ParsedNumberToken token)
        {
            PushValueOperandToken(token);
        }

        /// <summary>
        /// Callback when a variable token is read
        /// </summary>
        /// <param name="token"></param>
        protected override void OnVariableToken(ParsedVariableToken token)
        {
            PushVariableOperandToken(token);
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
        /// <summary>
        /// Pushes a Value Operand token
        /// </summary>
        /// <param name="t"></param>
        private void PushValueOperandToken(ParsedValueToken t)
        {
            var constantExpression = Expression.Constant(t.Value, t.Value.GetType());
            _evalTokens.Push(constantExpression);
        }

        /// <summary>
        /// Pushes a Variable Operand token
        /// </summary>
        /// <param name="t"></param>
        private void PushVariableOperandToken(ParsedVariableToken t)
        {
            var instance = Expression.Constant(_resolver, typeof(IIVariableResolver));
            var argsParam = new Expression[]
            {
                Expression.Constant(t.Name, typeof(string))
            };
            var varExpressionCall = Expression.Call(instance, _lazyVarProviderMethod.Value, argsParam);
            _evalTokens.Push(varExpressionCall);
        }
    }
}
