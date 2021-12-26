using System.Collections.Generic;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions
{
    /// <summary>
    /// Extension method for Dictionary<K, V>
    /// </summary>
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Add or update an existing key in a dictionary
        /// </summary>
        /// <typeparam name="TK">Key Type</typeparam>
        /// <typeparam name="TV">Value Type</typeparam>
        /// <param name="self"></param>
        /// <param name="key">Key to add or update</param>
        /// <param name="value">Value to add or update</param>
        public static void AddOrUpdateValue<TK, TV>(this IDictionary<TK, TV> self, TK key, TV value)
        {
            if (self.ContainsKey(key))
            {
                self[key] = value;
            }
            else
            {
                self.Add(key,value);
            }
        }
    }
}
