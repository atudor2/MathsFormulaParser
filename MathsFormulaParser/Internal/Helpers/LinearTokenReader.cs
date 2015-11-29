using System;
using System.Collections.Generic;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    /// <summary>
    /// Internal helper class for a linear read of Tokens
    /// </summary>
    internal class LinearTokenReader<T>
    {
        private readonly Queue<T> _tokenQueue;

        /// <summary>
        /// Constructs the reader from the IEnumerable source of tokens
        /// </summary>
        /// <param name="tokens"></param>
        public LinearTokenReader(IEnumerable<T> tokens)
        {
            this._tokenQueue = new Queue<T>(tokens);
        }

        /// <summary>
        /// Remaining number of tokens
        /// </summary>
        public int RemainingTokenCount => this._tokenQueue.Count;

        /// <summary>
        /// Are there any tokens left in the reader?
        /// </summary>
        public bool HasTokens => RemainingTokenCount > 0;

        /// <summary>
        /// Tries to read the next token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool TryReadNextToken(out T token)
        {
            if (!HasTokens)
            {
                token = default(T);
                return false;
            }
            token = this._tokenQueue.Dequeue();
            return true;
        }

        /// <summary>
        /// Reads the next token
        /// </summary>
        /// <exception cref="InvalidOperationException">Raised when there are no more tokens to read</exception>
        /// <returns></returns>
        public T ReadNextToken()
        {
            T returnToken;
            if (!TryReadNextToken(out returnToken))
            {
                throw new InvalidOperationException("End of Tokens!");
            }
            return returnToken;
        }
    }
}