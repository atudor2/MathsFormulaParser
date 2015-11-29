using System;
using System.Collections.Generic;
using System.Linq;
using Alistair.Tudor.MathsFormulaParser.Internal.Exceptions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Operators
{
    /// <summary>
    /// Represents an operator
    /// </summary>
    internal abstract class Operator
    {
        protected Operator(int precedence, string operatorSymbol, OperatorAssociativity associativity, int requiredNumberOfArguments, bool isSymbolicOperator = false)
        {
            Precedence = precedence;
            OperatorSymbol = operatorSymbol;
            Associativity = associativity;
            RequiredNumberOfArguments = requiredNumberOfArguments;
            IsSymbolicOperator = isSymbolicOperator;
        }

        /// <summary>
        /// Gets whether the given operator is 'symbolic' (i.e.: '1 + 2' vs '+(1, 2)'
        /// </summary>
        public bool IsSymbolicOperator { get; }

        /// <summary>
        /// Gets the Associativity or the operator
        /// </summary>
        public OperatorAssociativity Associativity { get; private set; }

        /// <summary>
        /// Gets the string representing the operator symbol (e.g. function name)
        /// </summary>
        public string OperatorSymbol { get; private set; }

        /// <summary>
        /// Gets the operator's precedence
        /// </summary>
        public int Precedence { get; private set; }
        /// <summary>
        /// Gets the required argument count
        /// </summary>
        public int RequiredNumberOfArguments { get; private set; }

        /// <summary>
        /// Gets or sets whether the input must be checked further before evaluation 
        /// </summary>
        public bool UseExtendedInputChecks { get; set; } = false;
        /// <summary>
        /// Runs the extended input checks for the input
        /// </summary>
        /// <param name="input"></param>
        public void CheckInput(double[] input)
        {
            InternalCheckInput(input, true);
        }

        /// <summary>
        /// Evaluates the operator with the given input
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="Exception">Raised if not enough arguments are given</exception>
        /// <returns></returns>
        public double Evaluate(double[] input)
        {
            AssertArgumentCount(input);

            var funcInput = input.Take(RequiredNumberOfArguments).ToArray();

            if (UseExtendedInputChecks)
            {
                // Extended checks:
                InternalCheckInput(input, false); // Already verified arg count
            }

            return InternalEvaluate(funcInput);
        }
        /// <summary>
        /// Gets the operator symbol
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return OperatorSymbol;
        }

        /// <summary>
        /// Internal method for implementing extended input checks
        /// </summary>
        /// <param name="input"></param>
        protected virtual void InternalCheckInput(double[] input)
        {
            // NOP
        }

        /// <summary>
        /// Internal method for evaluating the operator. Run after input checks have been executed
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected abstract double InternalEvaluate(double[] input);

        private void AssertArgumentCount<T>(IReadOnlyCollection<T> args)
        {
            if (!CheckCorrectArgCount(args))
            {
                throw new RpnEvaluationException($"Not enough arguments: Expected '{ RequiredNumberOfArguments }', got '{ args.Count }'");
            }
        }

        private bool CheckCorrectArgCount<T>(IReadOnlyCollection<T> input)
        {
            return !(input.Count < RequiredNumberOfArguments);
        }

        private void InternalCheckInput(double[] input, bool argCountCheck)
        {
            if (argCountCheck)
            {
                AssertArgumentCount(input);
            }
            var funcInput = input.Take(RequiredNumberOfArguments).ToArray();
            InternalCheckInput(funcInput);
        }
    }
}
