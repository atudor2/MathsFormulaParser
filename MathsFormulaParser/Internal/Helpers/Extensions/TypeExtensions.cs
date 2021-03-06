using System;
using System.Linq;
using System.Reflection;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions
{
    /// <summary>
    /// Type extensions
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Gets whether the given type is numerical
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        public static bool IsNumericType(this Type inputType)
        {
            ArgumentNullException.ThrowIfNull(inputType);

            switch (Type.GetTypeCode(inputType))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if a given MethodInfo is compatible with delegate type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsMethodCompatibleWithDelegate<T>(this MethodInfo method) where T : class
        {
            var delegateType = typeof(T);
            var delegateSignature = delegateType.GetMethod("Invoke");
            if (delegateSignature is null) return false;

            var parametersEqual = delegateSignature
                .GetParameters()
                .Select(x => x.ParameterType)
                .SequenceEqual(method.GetParameters()
                    .Select(x => x.ParameterType));

            return delegateSignature.ReturnType == method.ReturnType &&
                   parametersEqual;
        }
    }
}
