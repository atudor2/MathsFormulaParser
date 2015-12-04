using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Helpers
{
    /// <summary>
    /// Internal helper class for a linear read of Tokens
    /// </summary>
    internal class LinearTokenReader<T> : IEnumerable<T>
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
            return TryReadNextToken(out token, default(T));
        }

        /// <summary>
        /// Tries to read the next token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool TryReadNextToken(out T token, T defaultValue)
        {
            if (!HasTokens)
            {
                token = defaultValue;
                return false;
            }
            token = this._tokenQueue.Dequeue();
            return true;
        }

        /// <summary>
        /// Tries to peek ahead on the queue by the given number of items
        /// </summary>
        /// <remarks>
        /// This is a slow operation - use sparingly
        /// </remarks>
        /// <param name="items"></param>
        /// <returns></returns>
        public T[] TryPeekAhead(int items)
        {
            return _tokenQueue.Take(items).ToArray();
        }

        /// <summary>
        /// Tries to get the next item without removing it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return _tokenQueue.Peek();
        }

        /// <summary>
        /// Removes the given number of items from the reader
        /// </summary>
        /// <param name="items"></param>
        public void Remove(int items)
        {
            for (var i = 0; i < items; i++)
            {
                T nop;
                if (!TryReadNextToken(out nop))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the next item without removing it or returns the default value
        /// </summary>
        /// <returns></returns>
        public T TryPeek()
        {
            return TryPeek(default(T));
        }

        /// <summary>
        /// Gets the next item without removing it or returns the default value
        /// </summary>
        /// <returns></returns>
        public T TryPeek(T defaultValue)
        {
            return _tokenQueue.Count < 1 ? defaultValue : _tokenQueue.Peek();
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

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return _tokenQueue.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}