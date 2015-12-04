using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alistair.Tudor.MathsFormulaParser.Internal.Exceptions;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Functions
{
    internal class StandardFunction
    {
        /// <summary>
        /// Function string form
        /// </summary>
        private string _functionStringForm;

        public StandardFunction(string functionName, FormulaCallbackFunction callbackFunction, int requiredNumberOfArguments)
        {
            // Input checks:
            functionName.ThrowIfNull(nameof(functionName));
            callbackFunction.ThrowIfNull(nameof(callbackFunction));

            FunctionName = functionName;
            CallbackFunction = callbackFunction;
            RequiredNumberOfArguments = requiredNumberOfArguments;
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
                throw new CallbackFunctionException($"Not enough arguments: Expected '{ RequiredNumberOfArguments }', got '{ args.Count }'");
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
            // CHECK: If less then 0, no checks:
            if (RequiredNumberOfArguments < 0) return true;

            return !(input.Count < RequiredNumberOfArguments);
        }

        /// <summary>
        /// Callback delegate for the function
        /// </summary>
        public FormulaCallbackFunction CallbackFunction { get; }

        /// <summary>
        /// Gets the name of the function
        /// </summary>
        public virtual string FunctionName { get; }
        /// <summary>
        /// Gets the required argument count
        /// </summary>
        public virtual int RequiredNumberOfArguments { get; private set; }

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
        /// Gets the function name in a string form
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _functionStringForm ?? (_functionStringForm = MakeFunctionStringForm());
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
