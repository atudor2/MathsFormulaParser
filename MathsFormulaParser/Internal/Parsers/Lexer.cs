using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.LexicalAnalysis;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers
{
    /// <summary>
    /// Performs lexical analysis on the input formula
    /// </summary>
    internal class Lexer
    {
        /// <summary>
        /// List of Function 'buckets'
        /// This will map the start character of the Function to a list of operators
        /// E.g.:
        /// * -> *, **
        /// ( '*' maps to '*' and '**')
        /// </summary>
        private readonly Dictionary<char, string[]> _operatorBuckets = new Dictionary<char, string[]>();

        /// <summary>
        /// Global text source
        /// </summary>
        private readonly LinearTokenReader<char> _reader;

        /// <summary>
        /// List of the current run (WORD or NUMBER)
        /// </summary>
        private readonly Queue<char> _runBuilderQueue = new Queue<char>();

        /// <summary>
        /// Output queue of parsed tokens
        /// </summary>
        private readonly Queue<LexicalToken> _tokenQueue = new Queue<LexicalToken>();

        /// <summary>
        /// Current position of the character being read
        /// </summary>
        private long _currentCharacterPosition = -1;

        /// <summary>
        /// Current Lexer state
        /// </summary>
        private LexerState _currentLexerState = LexerState.Normal;

        /// <summary>
        /// Last processed character
        /// </summary>
        private char _lastCharacter;
        public Lexer(IEnumerable<char> input, IEnumerable<string> validOperatorSymbols)
        {
            _reader = new LinearTokenReader<char>(input);

            // Process the Function symbols:
            foreach (var symbol in validOperatorSymbols)
            {
                var firstChar = char.ToLower(symbol[0]);
                string[] currentOperators;
                if (_operatorBuckets.TryGetValue(firstChar, out currentOperators))
                {
                    _operatorBuckets[firstChar] = currentOperators.Concat(new[] { symbol }).ToArray();
                }
                else
                {
                    _operatorBuckets.Add(firstChar, new[] { symbol });
                }
            }
        }

        public Lexer(string input, IEnumerable<string> validOperatorSymbols) : this(input.ToCharArray(), validOperatorSymbols)
        {
        }
        /// <summary>
        /// Reads the input text from the reader and creates lexical tokens
        /// </summary>
        public void PerformLexicalAnalysis()
        {
            // TODO: Try move to using an input list of symbolic operators rather than hard coding the Function symbols?
            _tokenQueue.Clear();
            char character;
            while (ReadNextChar(out character))
            {
                _currentCharacterPosition++;
                if (HandlePreCheckRules(character))
                {
                    continue;
                }
                switch (character)
                {
                    // Start with the hard coded symbols:
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
                    default:
                        // Process the more complicated rules:
                        // Check whitespace
                        if (char.IsWhiteSpace(character))
                        {
                            HandleSpace(character);
                            break;
                        }
                        // Check if an Function:
                        if (HandleOperator(character))
                        {
                            break;
                        }
                        HandleFurtherRules(character);
                        break;
                }
                _lastCharacter = character;
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
        /// Adds an Function to the output tokens
        /// </summary>
        /// <param name="op"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddOperator(string op)
        {
            AddToken(LexicalTokenType.Operator, op);
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
            _tokenQueue.Enqueue(new LexicalToken(type, content, _currentCharacterPosition));
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
        /// 
        private void HandleFurtherRules(char character)
        {
            // This varies depending on our current state:
            // If we are a NUMBER and suddenly a 'char' appears, then flush the run 
            // and make a new word.
            // E.g.:
            // 1 2 3 A => Ends the number run at A, giving '123' & 'A'
            // A B C D 1 => The '1' joins the WORD run to give 'ABCD1'
            // Otherwise, we continue as a WORD until next flush (Function, space, etc)
            // However: Numbers can contain '.' and 'e/E'
            // E.g. 2.3e10 => 2.3 x 10^(10)
            // Therefore: To check if we the char is a valid number, we may need to peek ahead in the stream to double check
            // that the next char is a digit. BUT: This can only happen when already in a number run to
            // reject input like: e1 .1 etc

            // Naive expected state: This is the state we expect based purely on the char and number rules,
            // therefore ignoring the WORD run rule.
            var expectedState = IsNextExpectedStateNumber(character) ? LexerState.NumberRun : LexerState.WordRun;

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
        /// Tries to handle the case where the read character is/part of an Function
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool HandleOperator(char character)
        {
            // Get the bucket for the character:
            string[] operatorsBucket;
            if (!_operatorBuckets.TryGetValue(character, out operatorsBucket))
            {
                // Not found!
                return false;
            }

            Debug.Assert(operatorsBucket.Length > 0, "operatorsBucket has no items!");

            // TODO: Optimise this to be one loop through? Use a HASHSET for direct lookups?

            // Find the maximum length Function in the set:
            var maxOpLength = operatorsBucket.Max(s => s.Length);

            if (maxOpLength == 1)
            {
                // Shortcut: We can just directly search without look ahead
                // Direct search for the item:
                var op = operatorsBucket.FirstOrDefault(o => string.Equals(o, char.ToLower(character).ToString(), StringComparison.InvariantCultureIgnoreCase));
                if (op != null)
                {
                    AddOperator(op);
                    return true;
                }
                return false;
            }

            maxOpLength--; // Less one because we already have it (character)

            // Peek ahead on the queue by the longest potential operator
            // STOP on first whitespace - whitespace ALWAYS terminates an operator run 
            var readAheadStr = character + new string(_reader.TryPeekAhead(maxOpLength).TakeWhile(IsValidOperatorCharacter).ToArray());

            // Try match the operators, starting with the longest:
            for (var i = readAheadStr.Length; i > 0; i++)
            {
                if (operatorsBucket.Contains(readAheadStr))
                {
                    AddOperator(readAheadStr);
                    // Remove the peeked characters
                    if (i != 1)
                        _reader.Remove(i - 1);
                    return true;
                }
                if (i != 1)
                {
                    // Lop off a char and try again:
                    readAheadStr = SafeSubstring(readAheadStr, 0, i - 1);
                }
            }

            // Nothing found
            return false;
        }

        /// <summary>
        /// Handles rules for characters run before standard riles
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool HandlePreCheckRules(char character)
        {
            // Only if '-' and if in number run and last char 'E' and next char is number
            if (character != '-') return false;
            if (_currentLexerState != LexerState.NumberRun) return false;
            if (char.ToLower(_lastCharacter) != 'e') return false;
            if (!char.IsDigit(_reader.TryPeek('\0'))) return false;

            _runBuilderQueue.Enqueue(character);
            return true;
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
        /// <returns></returns>
        private bool IsNextExpectedStateNumber(char character)
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
                    // Only valid if next char is an integer or '-':
                    var nextChar = TryPeek();
                    return char.IsDigit(nextChar) || nextChar == '-'; // Is it a digit or '-'?
            }
            return false;
        }

        /// <summary>
        /// Returns TRUE if the given char is a valid Function symbol
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsValidOperatorCharacter(char c)
        {
            return !char.IsWhiteSpace(c) && !char.IsNumber(c);
        }

        /// <summary>
        /// Reads the next character or returns 'char.MinValue'
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool ReadNextChar(out char character)
        {
            return _reader.TryReadNextToken(out character);
        }

        private string SafeSubstring(string input, int start, int end)
        {
            if (start < 0) start = 0;
            if (end > input.Length) end = input.Length;
            return input.Substring(start, end);
        }
        /// <summary>
        /// Tries to peek at the next character from the source
        /// </summary>
        /// <param name="peekCount">Number of items to peek</param>
        /// <param name="defaultChar"></param>
        /// <returns></returns>
        private char TryPeek(char defaultChar = '\0')
        {
            return _reader.TryPeek(defaultChar);
        }
    }
}
