﻿using System.Collections.Generic;
using System.Linq;
using Alistair.Tudor.MathsFormulaParser.Internal.Exceptions;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators
{
    /// <summary>
    /// Abstract base class for a Reverse Polish Notation Evaluator. 
    /// Contains the basic parsing and evaluation of operators with 
    /// virtual callback methods for subclasses to handle when a particular token 
    /// is reached
    /// </summary>
    internal abstract class AbstractRpnEvaluator
    {
        /// <summary>
        /// Token reader source
        /// </summary>
        private LinearTokenReader<ParsedToken> _reader;

        /// <summary>
        /// Should extended checks be run when executing operators?
        /// </summary>
        public virtual bool PerformExtendedChecks { get; set; } = false;

        protected AbstractRpnEvaluator(ParsedToken[] tokens)
        {
            Tokens = tokens;
            this.Reset();
        }

        /// <summary>
        /// Gets whether there are more tokens to be read
        /// </summary>
        public bool HasTokens => _reader.HasTokens;

        /// <summary>
        /// Current items on the evaluation stack
        /// </summary>
        protected Stack<double> EvaluatorStack { get; } = new Stack<double>();

        /// <summary>
        /// Gets the the tokens being evaluated
        /// </summary>
        protected IReadOnlyCollection<ParsedToken> Tokens { get; private set; }

        /// <summary>
        /// Tries to get the result of the evaluation. Throws an exception if not completely evaluated!
        /// </summary>
        /// <returns></returns>
        public virtual double GetResult()
        {
            // Check stack: if only 1 item => Result
            // Otherwise, syntax error!
            if (EvaluatorStack.Count != 1)
            {
                RaiseError(null, "Syntax error: Too many items left on stack!");
            }
            var result = EvaluatorStack.Pop();
            return result;
        }

        /// <summary>
        /// Reads the next token
        /// </summary>
        public void ReadNextToken()
        {
            var token = _reader.ReadNextToken();
            if (token is ParsedNumberToken)
            {
                OnNumberToken(token.CastTo<ParsedNumberToken>());
            }
            else if (token is ParsedConstantToken)
            {
                OnConstantToken(token.CastTo<ParsedConstantToken>());
            }
            else if (token is ParsedVariableToken)
            {
                OnVariableToken(token.CastTo<ParsedVariableToken>());
            }
            else if (token is ParsedFunctionToken)
            {
                HandleOperator(token.CastTo<ParsedFunctionToken>());
            }
            else
            {
                RaiseError(token, $"Internal Error: Unexpected token type '{ token.GetType().Name }'");
            }
        }

        /// <summary>
        /// Resets the evaluator
        /// </summary>
        public void Reset()
        {
            EvaluatorStack.Clear();
            _reader = new LinearTokenReader<ParsedToken>(Tokens);
        }

        /// <summary>
        /// Evaluates the given Function and it's arguments
        /// </summary>
        /// <param name="function"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected virtual double EvaluateOperator(Function function, double[] arguments)
        {
            // Run the handler function:
            var opResult = function.Evaluate(arguments);
            return opResult;
        }

        /// <summary>
        /// Extracts the Function from the token, verifies the correct number of arguments are available on the EvaluatorStack
        /// and exports the arguments for evaluation
        /// </summary>
        /// <param name="token"></param>
        /// <param name="function"></param>
        /// <param name="arguments"></param>
        protected virtual void ExtractAndVerifyOperatorInfo(ParsedFunctionToken token, out Function function, out double[] arguments)
        {
            var t = token;
            function = t.Function;
            var argCount = function.RequiredNumberOfArguments;
            if (argCount > EvaluatorStack.Count)
            {
                // Not enough args!
                RaiseError(token, $"Not enough arguments for '{ t.Value }': Expected '{ argCount  }', but only '{ EvaluatorStack.Count }' are available");
            }
            // Pop off the args:
            arguments = EvaluatorStack.PopOff(argCount).Reverse().ToArray();
        }

        /// <summary>
        /// Callback when a constant token is read
        /// </summary>
        /// <param name="token"></param>
        protected virtual void OnConstantToken(ParsedConstantToken token)
        {
            PushToStack(token.Value);
        }

        /// <summary>
        /// Callback when a number token is read
        /// </summary>
        /// <param name="token"></param>
        protected virtual void OnNumberToken(ParsedNumberToken token)
        {
            PushToStack(token.Value);
        }

        /// <summary>
        /// Callback when an Function token is read
        /// </summary>
        /// <param name="token"></param>
        /// <returns>True if the Function should be evaluated or False to skip</returns>
        protected virtual bool OnOperatorToken(ParsedFunctionToken token)
        {
            // Set the extended check flag
            SetOperatorExtendedCheckFlag(token);
            // Nothing else we have to do here....
            return true;
        }

        /// <summary>
        /// Sets the Function extended check flag as it is encountered
        /// </summary>
        /// <param name="token"></param>
        protected virtual void SetOperatorExtendedCheckFlag(ParsedFunctionToken token)
        {
            token.Function.UseExtendedInputChecks = PerformExtendedChecks;
        }

        /// <summary>
        /// Callback when a variable token is read
        /// </summary>
        /// <param name="token"></param>
        protected virtual void OnVariableToken(ParsedVariableToken token)
        {
            // Resolve the value:
            var varValue = ResolveVariable(token.Name);
            PushToStack(varValue);
        }

        /// <summary>
        /// Raises an error
        /// </summary>
        /// <param name="token"></param>
        /// <param name="msg"></param>
        protected virtual void RaiseError(ParsedToken token, string msg)
        {
            throw new RpnEvaluationException(msg);
        }

        /// <summary>
        /// Resolves a variable name to its value
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract double ResolveVariable(string name);

        /// <summary>
        /// Handles a given variable
        /// </summary>
        /// <param name="token"></param>
        private void HandleOperator(ParsedFunctionToken token)
        {
            if (!OnOperatorToken(token.CastTo<ParsedFunctionToken>())) return; // STOP
            Function function;
            double[] args;
            ExtractAndVerifyOperatorInfo(token, out function, out args);
            var result = EvaluateOperator(function, args);
            PushToStack(result);
        }

        /// <summary>
        /// Pushes the value to the evaluation stack
        /// </summary>
        /// <param name="token"></param>
        private void PushToStack(double token)
        {
            EvaluatorStack.Push(token);
        }
    }
}