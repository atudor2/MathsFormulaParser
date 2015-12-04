using System;
using System.Collections.Generic;
using Alistair.Tudor.MathsFormulaParser.Internal.FormulaEvaluators;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.RpnEvaluators;

namespace Alistair.Tudor.MathsFormulaParser.Internal
{
    /// <summary>
    /// Helper class to optimise formulae
    /// </summary>
    internal static class FormulaOptimiser
    {
        /// <summary>
        /// Optimises the given list of tokens and to the given level
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static IInternalEvaluator OptimiseFormula(ParsedToken[] tokens, FormulaOptimisationLevel level)
        {
            switch (level)
            {
                case FormulaOptimisationLevel.None:
                    return NoOptimisation(tokens);
                case FormulaOptimisationLevel.Basic:
                    return BasicOptimisation(tokens);
                case FormulaOptimisationLevel.Compiled:
                    return CompiledOptimisation(tokens);
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        /// <summary>
        /// Performs BASIC optimisation
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private static IInternalEvaluator BasicOptimisation(ParsedToken[] tokens)
        {
            // Simply optimise the tokens and return a normal evaluator:
            var optTokens = BasicTokenOptimisation(tokens);
            return NoOptimisation(optTokens);
        }

        /// <summary>
        /// Flattens all constant expressions
        /// </summary>
        /// <returns></returns>
        private static ParsedToken[] BasicTokenOptimisation(ParsedToken[] tokens)
        {
            var rpnOptimiser = new RpnOptimiser(tokens);
            return rpnOptimiser.OptimiseExpression();
        }

        /// <summary>
        /// Performs COMPILED optimisation by generating direct operator method calls
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private static IInternalEvaluator CompiledOptimisation(ParsedToken[] tokens)
        {
            // Get the optimised tokens to reduce generated method calls:
            var newTokens = BasicTokenOptimisation(tokens);

            // Start creating the code:
            // t.k.
            throw new NotImplementedException("Implementation not complete");
        }

        /// <summary>
        /// Performs NO optimisation
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private static IInternalEvaluator NoOptimisation(ParsedToken[] tokens)
        {
            // Just return a standard evaluator for all the tokens
            return new NormalTokenEvaluator(tokens);
        }
    }
}
