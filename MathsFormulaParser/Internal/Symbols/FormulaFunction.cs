using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alistair.Tudor.MathsFormulaParser.Exceptions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Symbols
{
    /// <summary>
    /// Base class for all callable functions or operators
    /// </summary>
    internal abstract class FormulaFunction : IFunction
    {
        /// <summary>
        /// Function string form
        /// </summary>
        private Lazy<string> _functionStringForm;

        /// <summary>
        /// Backing for function name
        /// </summary>
        private string _functionName;

        protected FormulaFunction(string functionName, FormulaCallbackFunction callbackFunction, int requiredNumberOfArguments)
        {
            // Input checks:
            ArgumentNullException.ThrowIfNull(functionName);
            ArgumentNullException.ThrowIfNull(callbackFunction);

            if (requiredNumberOfArguments < 0) throw new ArgumentOutOfRangeException(nameof(requiredNumberOfArguments), "Required argument count must be >= 0");

            _functionName = functionName; // Avoid virtual member access in .ctor()
            _functionStringForm = new Lazy<string>(MakeFunctionStringForm);

            CallbackFunction = callbackFunction;
            RequiredNumberOfArguments = requiredNumberOfArguments;
        }

        /// <summary>
        /// Callback delegate for the function
        /// </summary>
        public FormulaCallbackFunction CallbackFunction { get; }

        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public virtual string FunctionName
        {
            get => _functionName;
            protected set => _functionName = value;
        }

        /// <summary>
        /// Gets the required argument count
        /// </summary>
        public virtual int RequiredNumberOfArguments { get; private set; }

        /// <summary>
        /// Checks if the given input is the correct number of arguments
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool CheckCorrectArgumentCount(int arguments)
        {
            return arguments >= RequiredNumberOfArguments;
        }

        /// <summary>
        /// Evaluates the function with the given input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double Evaluate(double[] input)
        {
            AssertArgumentCount(input);
            var funcInput = input.Take(RequiredNumberOfArguments).ToArray();
            return InternalEvaluate(funcInput);
        }

        /// <summary>
        /// Serves as the default hash function. 
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                hash = hash * 23 + FunctionName.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Returns a 'pretty' string form of the function - e.g. f(a, b)
        /// </summary>
        /// <returns></returns>
        public virtual string GetPrettyFunctionString()
        {
            return _functionStringForm.Value;
        }

        /// <summary>
        /// Renames the current function
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public virtual FormulaFunction RenameFunction(string newName)
        {
            FunctionName = newName;
            return this;
        }

        /// <summary>
        /// Gets the function name in a string form
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FunctionName;
        }

        /// <summary>
        /// Internal method for evaluating the Function. 
        /// Run after input checks have been executed (e.g. argument count check etc)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected virtual double InternalEvaluate(double[] input)
        {
            // Simply call the delegate
            return CallbackFunction(input);
        }

        /// <summary>
        /// Asserts that the enough arguments are given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        private void AssertArgumentCount<T>(IReadOnlyCollection<T> args)
        {
            if (!CheckCorrectArgCount(args))
            {
                throw new FormulaCallbackFunctionException($"Not enough arguments: Expected '{ RequiredNumberOfArguments }', got '{ args.Count }'");
            }
        }
        /// <summary>
        /// Returns TRUE if the number of items in the input is >= RequiredNumberOfArguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool CheckCorrectArgCount<T>(IReadOnlyCollection<T> input)
        {
            return !(input.Count < RequiredNumberOfArguments);
        }
        /// <summary>
        /// Creates a string form of the function
        /// E.g. func(a, b, c)
        /// </summary>
        /// <returns></returns>
        private string MakeFunctionStringForm()
        {
            var builder = new StringBuilder();
            builder.Append($"{FunctionName}(");
            if (RequiredNumberOfArguments != 0)
            {
                var startChar = 'a';
                for (var i = 0; i < RequiredNumberOfArguments; i++)
                {
                    if (i != 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append($"{startChar++}");
                }
            }
            builder.Append(")");
            return builder.ToString();
        }
    }
}
