using System;
using Alistair.Tudor.Utility.Extensions;

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
            inputType.ThrowIfNull(nameof(inputType));

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
    }
}
