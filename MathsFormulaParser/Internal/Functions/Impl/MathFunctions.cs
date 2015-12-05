using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Functions.Impl
{
    /// <summary>
    /// Class used to load wrappers for System.Math functions
    /// </summary>
    internal static class MathFunctions
    {
        /// <summary>
        /// Gets a list of wrappers for valid System.Math functions
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Function> GetFunctionWrappersForMath()
        {
            var mathType = typeof(Math);
            var methods = mathType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods.Where(CheckMathLibMethod))
            {
                var @params = method.GetParameters().Length;

                // Make an Function:
                // We REALLY want to avoid making a dynamic Invoke() call at runtime
                // It can be 500x slower!
                // But creating a dynamic Func<> for a delegate doesn't help as we would still then need to 
                // Invoke() the delegate
                // So:
                // Handle a set number of arities:
                // 0, 1, 2, 3
                // Everything else will be dynamically invoked:

                FormulaCallbackFunction thunk;

                switch (@params)
                {
                    case 0:
                        thunk = Make0ArityThunk(method);
                        break;
                    case 1:
                        thunk = Make1ArityThunk(method);
                        break;
                    case 2:
                        thunk = Make2ArityThunk(method);
                        break;
                    case 3:
                        thunk = Make3ArityThunk(method);
                        break;
                    default:
                        // Fallback to Invoke()
                        thunk = (i) => (double)method.Invoke(null, i.Select(x => (object)x).ToArray());
                        break;
                }

                var name = method.Name.ToLower();

                var op = new Function(name, thunk, @params);
                yield return op;
            }
        }

        /// <summary>
        /// 3 Arity function thunk
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static FormulaCallbackFunction Make3ArityThunk(MethodInfo method)
        {
            var func = (Func<double, double, double, double>)Delegate.CreateDelegate(typeof(Func<double, double, double, double>), method);
            return (i) => func(i[0], i[1], i[2]);
        }

        /// <summary>
        /// 2 Arity function thunk
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static FormulaCallbackFunction Make2ArityThunk(MethodInfo method)
        {
            var func = (Func<double, double, double>)Delegate.CreateDelegate(typeof(Func<double, double, double>), method);
            return (i) => func(i[0], i[1]);
        }

        /// <summary>
        /// 1 Arity function thunk
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static FormulaCallbackFunction Make1ArityThunk(MethodInfo method)
        {
            var func = (Func<double, double>)Delegate.CreateDelegate(typeof(Func<double, double>), method);
            return (i) => func(i[0]);
        }

        /// <summary>
        /// 0 Arity function thunk
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static FormulaCallbackFunction Make0ArityThunk(MethodInfo method)
        {
            var func = (Func<double>)Delegate.CreateDelegate(typeof(Func<double>), method);
            return (i) => func();
        }

        /// <summary>
        /// Checks if the given method is a valid System.Math method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static bool CheckMathLibMethod(MethodInfo method)
        {
            var validReturnType = method.ReturnType == typeof(double);
            var validParams = method.GetParameters().All(p => p.ParameterType == typeof(double));

            return validParams && validReturnType;
        }
    }
}
