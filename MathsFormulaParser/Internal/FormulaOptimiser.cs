using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvalutors;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators;

namespace Alistair.Tudor.MathsFormulaParser.Internal
{
    /// <summary>
    /// Helper class for optimising sets of rpn tokens and returning evaluators
    /// </summary>
   internal static class FormulaOptimiser
    {
        /// <summary>
        /// Creates an IInternalFormulaEvaluator without any optimisation
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        public static IInternalFormulaEvaluator NoOptimisation(ParsedToken[] tokens)
        {
            // Nothing except return the evaulator
            return new NormalFormulaEvaluator(tokens);
        }

        /// <summary>
        /// Creates an IInternalFormulaEvaluator with BASIC optimisation (i.e. flattening of constant expression etc)
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        public static IInternalFormulaEvaluator BasicOptimisation(ParsedToken[] tokens)
        {
            var optimisedTokens = OptimiseTokens(tokens);
            // Return the evaulator
            return new NormalFormulaEvaluator(optimisedTokens);
        }

        /// <summary>
        /// Creates an IInternalFormulaEvaluator with COMPILED optimisation (i.e. flattening of constant expression and compiled)
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static IInternalFormulaEvaluator CompiledOptimisation(ParsedToken[] tokens)
        {
            // Still optimise tokens to remove constant expressions:
            var optimisedTokens = OptimiseTokens(tokens);
            // Return the evaulator
            return new CompiledFormulaEvaluator(optimisedTokens);
        }

        /// <summary>
        /// Optimises the tokens by pre-evaluating constants
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="formula"></param>
        /// <returns></returns>
        private static ParsedToken[] OptimiseTokens(ParsedToken[] tokens)
        {
            var optimiser = new RpnOptimiser(tokens);
            return optimiser.OptimiseExpression();
        }
    }
}
