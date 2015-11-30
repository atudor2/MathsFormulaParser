using System;
using System.Collections.Generic;
using System.IO;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.LexicalAnalysis;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers
{
    /// <summary>
    /// Performs lexical analysis on the input formula
    /// </summary>
    internal class Lexer
    {
        /// <summary>
        /// Global text source
        /// </summary>
        private readonly TextReader _reader;
    
        /// <summary>
        /// List of the current run (WORD or NUMBER)
        /// </summary>
        private readonly Queue<char> _runBuilderQueue = new Queue<char>();

        /// <summary>
        /// Output queue of parsed tokens
        /// </summary>
        private readonly Queue<LexicalToken> _tokenQueue = new Queue<LexicalToken>();

        /// <summary>
        /// Current Lexer state
        /// </summary>
        private LexerState _currentLexerState = LexerState.Normal;

        public Lexer(TextReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// Reads the input text from the reader and creates lexical tokens
        /// </summary>
        public void PerformLexicalAnalysis()
        {
            // TODO: Try move to using an input list of symbolic operators rather than hard coding the operator symbols?
            _tokenQueue.Clear();
            char character;
            while (ReadNextChar(out character))
            {
                switch (character)
                {
                    case '(':
                        AddToken(LexicalTokenType.StartSubExpression, character);
                        break;
                    case ')':
                        AddToken(LexicalTokenType.EndSubExpression, character);
                        break;
                    case '[':
                        AddToken(LexicalTokenType.StartSubScript, character);
                        break;
                    case ']':
                        AddToken(LexicalTokenType.EndSubScript, character);
                        break;
                    case ',':
                        AddToken(LexicalTokenType.Comma, ',');
                        break;
                    case '+':
                    case '-':
                    case '/':
                    case '^':
                    case '|':
                    case '&':
                    case '~':
                    case '%':
                        AddToken(LexicalTokenType.Operator, character);
                        break;
                    case '*':
                        HandleMultiplicationOrPowerOp();
                        break;
                    case '>':
                    case '<':
                        if (!HandlePotentialBitShift(character))
                        {
                            goto default;
                        }
                        break;
                    default:
                        // Process the more complicated rules:
                        if (char.IsWhiteSpace(character))
                        {
                            HandleSpace(character);
                            break;
                        }
                        HandleFurtherRules(character, _reader);
                        break;
                }
            }
            FlushRuns();
        }

        /// <summary>
        /// Gets the Lexical Tokens that have been parsed
        /// </summary>
        /// <returns></returns>
        internal LexicalToken[] GetTokens()
        {
            return _tokenQueue.ToArray();
        }

        /// <summary>
        /// Adds a token to the queue and flushes any runs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="flushRuns"></param>
        private void AddToken(LexicalTokenType type, char content, bool flushRuns = true)
        {
            AddToken(type, content.ToString(), flushRuns);
        }

        /// <summary>
        /// Adds a token to the queue and flushes any runs
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="flushRuns"></param>
        private void AddToken(LexicalTokenType type, string content, bool flushRuns = true)
        {
            // If in word or number, this ends the run:
            if (flushRuns)
            {
                FlushRuns();
            }
            _tokenQueue.Enqueue(new LexicalToken(type, content));
        }

        /// <summary>
        /// Flushes a run
        /// </summary>
        private void FlushRuns()
        {
            switch (_currentLexerState)
            {
                case LexerState.Normal:
                    return; // Nothing to do
                case LexerState.NumberRun:
                case LexerState.WordRun:
                    // End the previous run:
                    var runText = string.Join("", _runBuilderQueue);
                    _runBuilderQueue.Clear();
                    var tokenType = _currentLexerState == LexerState.NumberRun ? LexicalTokenType.Number : LexicalTokenType.Word;
                    AddToken(tokenType, runText, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currentLexerState = LexerState.Normal;
        }

        /// <summary>
        /// Handles the further rules: Number runs or Word runs
        /// </summary>
        /// <param name="character"></param>
        /// <param name="reader"></param>
        private void HandleFurtherRules(char character, TextReader reader)
        {
            // This varies depending on our current state:
            // If we are a NUMBER and suddenly a 'char' appears, then flush the run 
            // and make a new word.
            // E.g.:
            // 1 2 3 A => Ends the number run at A, giving '123' & 'A'
            // A B C D 1 => The '1' joins the WORD run to give 'ABCD1'
            // Otherwise, we continue as a WORD until next flush (operator, space, etc)
            // However: Numbers can contain '.' and 'e/E'
            // E.g. 2.3e10 => 2.3 x 10^(10)
            // Therefore: To check if we the char is a valid number, we may need to peek ahead in the stream to double check
            // that the next char is a digit. BUT: This can only happen when already in a number run to
            // reject input like: e1 .1 etc

            // Naive expected state: This is the state we expect based purely on the char and number rules,
            // therefore ignoring the WORD run rule.
            var expectedState = IsNextExpectedStateNumber(character, reader) ? LexerState.NumberRun : LexerState.WordRun;

            if (_currentLexerState == LexerState.Normal)
            {
                // Moving to a new state...
                _currentLexerState = expectedState;
            }

            switch (_currentLexerState)
            {
                case LexerState.WordRun:
                    // Just add it onto the end:
                    _runBuilderQueue.Enqueue(character);
                    break;
                case LexerState.NumberRun:

                    if (_currentLexerState != expectedState)
                    {
                        // Need to flush:
                        FlushRuns();
                    }

                    _runBuilderQueue.Enqueue(character);
                    _currentLexerState = expectedState;
                    break;
            }
        }

        /// <summary>
        /// Handles a multiplication or Power operator
        /// </summary>
        private void HandleMultiplicationOrPowerOp()
        {
            // Special case: Check if ** (power operator)
            var nextChar = TryPeek(_reader);
            string @operator;
            if (nextChar != '\0' && nextChar == '*')
            {
                // Power operator:
                @operator = "**";
                // Take next char
                _reader.Read();
            }
            else
            {
                // Bog standard multiplication operator
                @operator = "*";
            }
            AddToken(LexicalTokenType.Operator, @operator);
        }

        /// <summary>
        /// Tries to handle a potential bit shift operator
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool HandlePotentialBitShift(char character)
        {
            // Special case: operator is >> or <<
            var nextChar = TryPeek(_reader);
            if (nextChar != '\0')
            {
                if (character == nextChar)
                {
                    AddToken(LexicalTokenType.Operator, string.Concat(character, nextChar));
                    _reader.Read(); // Skip next char
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handles a whitespace character
        /// </summary>
        /// <param name="character"></param>
        private void HandleSpace(char character)
        {
            AddToken(LexicalTokenType.Space, character);
        }

        /// <summary>
        /// Checks if the next expected state is LexerState.NumberRun
        /// </summary>
        /// <remarks>This will check for 2.2e3 etc</remarks>
        /// <param name="character"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private bool IsNextExpectedStateNumber(char character, TextReader reader)
        {
            if (char.IsDigit(character))
            {
                // Easy: It's a digit
                return true;
            }
            // Now for the more difficult rules:
            // Only check if in a number run!
            if (_currentLexerState != LexerState.NumberRun) return false; // Not a number
            switch (character)
            {
                case '.': // Decimal sep.
                case 'e':
                case 'E': // Exponential 
                    // Only valid if next char is an integer:
                    var nextChar = TryPeek(reader);
                    return char.IsDigit(nextChar); // Is it a digit?
            }
            return false;
        }

        /// <summary>
        /// Reads the next character or returns 'char.MinValue'
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool ReadNextChar(out char character)
        {
            var nextChar = _reader.Read();
            if (nextChar >= 0)
            {
                character = (char)nextChar;
                return true;
            }
            character = char.MinValue;
            return false;
        }

        /// <summary>
        /// Tries to peek at the next character from the TextReader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="defaultChar"></param>
        /// <returns></returns>
        private char TryPeek(TextReader reader, char defaultChar = '\0')
        {
            var nextCharInt = reader.Peek();
            return nextCharInt < 0 ? defaultChar : (char)nextCharInt;
        }
    }
}
