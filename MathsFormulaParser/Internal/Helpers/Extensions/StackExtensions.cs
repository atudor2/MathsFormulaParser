using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions
{
    /// <summary>
    /// Extension method for Stack
    /// </summary>
    internal static class StackExtensions
    {
        /// <summary>
        /// Tries to pop and item off the stack. If the stack is empty, defaultValue is returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool TryPop<T>(this Stack<T> stack, out T? value, T? defaultValue = default(T))
        {
            ArgumentNullException.ThrowIfNull(stack);
            if (stack.Any())
            {
                value = stack.Pop();
                return true;
            }

            value = defaultValue;
            return false;
        }

        /// <summary>
        /// Pops off the given number of items off the stack
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> PopOff<T>(this Stack<T> stack, int count)
        {
            ArgumentNullException.ThrowIfNull(stack);
            for (var i = 0; i < count; i++)
            {
                yield return stack.Pop();
            }
        }

        /// <summary>
        /// Tries to peek at the next item on the stack or returns defaultValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool TryPeek<T>(this Stack<T> stack, out T? value, T? defaultValue = default(T))
        {
            ArgumentNullException.ThrowIfNull(stack);

            if (stack.Any())
            {
                value = stack.Peek();
                return true;
            }

            value = defaultValue;
            return false;
        }
    }
}
