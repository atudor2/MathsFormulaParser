using System.Collections.Generic;
using System.Linq;
using Alistair.Tudor.Utility.Extensions;

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
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T TryPop<T>(this Stack<T> stack, T defaultValue = default(T))
        {
            stack.ThrowIfNull(nameof(stack));
            return stack.Any() ? stack.Pop() : defaultValue;
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
            stack.ThrowIfNull(nameof(stack));
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
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T TryPeek<T>(this Stack<T> stack, T defaultValue = default(T))
        {
            stack.ThrowIfNull(nameof(stack));
            return stack.Any() ? stack.Peek() : defaultValue;
        }
    }
}
