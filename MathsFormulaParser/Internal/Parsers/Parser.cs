using System;
using System.Collections.Generic;
using System.Globalization;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators;
using Alistair.Tudor.MathsFormulaParser.Internal.Operators.Impl;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.LexicalAnalysis;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.Utility.Extensions;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers
{
    /// <summary>
    /// Parses the lexical tokens to ParsedTokens
    /// </summary>
    internal class Parser
    {
        /// <summary>
        /// Dictionary of constants
        /// </summary>
        private readonly Dictionary<string, double> _constantsDictionary = new Dictionary<string, double>()
        {
            { "PI", Math.PI },
            { "EU", Math.E }
        };

        /// <summary>
        /// Dictionary of registered operators
        /// </summary>
        private readonly Dictionary<string, Operator> _operatorsDictionary = new Dictionary<string, Operator>()
        {
             // Pseudo operator - not used in output!
            {"(", new GenericOperator(int.MaxValue, "(", OperatorAssociativity.Left, 0, (i) => { throw new InvalidOperationException("Cannot Invoke the '(' pseudo operator!"); })},
        };

        /// <summary>
        /// Input list of lexical tokens
        /// </summary>
        private readonly LexicalToken[] _tokens;

        /// <summary>
        /// Current parser state
        /// </summary>
        private ParserState _currentParserState = ParserState.Normal;

        /// <summary>
        /// Output list of parsed tokens
        /// </summary>
        private ParsedToken[] _rpnTokens;


        public Parser(LexicalToken[] tokens, IDictionary<string, Operator> operatorsDictionary, IDictionary<string, double> customConstantsMap = null)
        {
            _tokens = tokens;

            if (customConstantsMap != null)
            {
                foreach (var map in customConstantsMap)
                {
                    _constantsDictionary.AddOrUpdateValue(map.Key, map.Value);
                }
            }

            if (customConstantsMap != null)
            {
                foreach (var op in operatorsDictionary)
                {
                    _operatorsDictionary.AddOrUpdateValue(op.Key, op.Value);
                }
            }
        }

        /// <summary>
        /// Gets a list of the parsed tokens
        /// </summary>
        /// <returns></returns>
        public ParsedToken[] GetReversePolishNotationTokens()
        {
            return _rpnTokens;
        }

        /// <summary>
        /// Parses the list of lexical tokens
        /// </summary>
        public void ParseTokens()
        {
            var tokenReader = new LinearTokenReader<LexicalToken>(_tokens);

            _rpnTokens = ConvertToReversePolishNotation(tokenReader);
        }
        
        /// <summary>
        /// Attempts to change the parser state. Raises parser error if the expected state != current state
        /// </summary>
        /// <param name="newParserState"></param>
        /// <param name="expectedState"></param>
        private void ChangeParserStart(ParserState newParserState, ParserState expectedState)
        {
            if (_currentParserState != expectedState)
            {
                RaiseParserError(null, $"Unexpected parser state: Expected '{ Enum.GetName(typeof(ParserState), expectedState)  }', but found '{ Enum.GetName(typeof(ParserState), _currentParserState) }'");
                return;
            }
            _currentParserState = newParserState;
        }

        /// <summary>
        /// Converts the lexical tokens into Reverse Polish Notation parsed tokens
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private ParsedToken[] ConvertToReversePolishNotation(LinearTokenReader<LexicalToken> reader)
        {
            var outputQueue = new Queue<ParsedToken>();
            var operatorStack = new Stack<Operator>();
            var holderStruct = new RpnHolderStruct(outputQueue, operatorStack);

            // Implementation of the "Shunting-yard Algorithm"
            // We are going to convert an infix format mathematical formula to
            // Reverse Polish Notation (RPN).
            // RPN is the...reverse of Polish Notation: + 2 2 => 2 + 2
            // Therefore: 2 2 +  => 2 + 2
            // 3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3 => 3 4 2 * 1 5 - 2 3 ^ ^ / +
            while (reader.HasTokens)
            {
                var token = reader.ReadNextToken();
                switch (token.TokenType)
                {
                    case LexicalTokenType.Space:
                        continue; // Ignore
                    case LexicalTokenType.Number:
                        ValidateTokenHasValue(token);
                        // Add to the output queue:
                        outputQueue.Enqueue(new ParsedNumberToken(double.Parse(token.Value, CultureInfo.InvariantCulture)));
                        break;
                    case LexicalTokenType.Word: // Variable or constant or function
                        ValidateTokenHasValue(token);
                        HandleWord(holderStruct, token);
                        break;
                    case LexicalTokenType.Comma:
                        HandleComma(holderStruct, token);
                        break;
                    case LexicalTokenType.Operator:
                        HandleOperator(holderStruct, token);
                        break;
                    case LexicalTokenType.StartSubScript:
                        // This is == to the function 'GetBit(number, bitPos)'
                        // Therefore push the 'GetBit()' function at END of the subscript!
                        // Mark this as our parser state:
                        ChangeParserStart(ParserState.InSubScript, ParserState.Normal);
                        break;
                    case LexicalTokenType.EndSubScript:
                        ChangeParserStart(ParserState.Normal, ParserState.InSubScript);
                        operatorStack.Push(_operatorsDictionary["getbit"]); // Special op - guaranteed to be there!
                        break;
                    case LexicalTokenType.StartSubExpression:
                        // '('
                        operatorStack.Push(_operatorsDictionary["("]); // Special op - guaranteed to be there!
                        break;
                    case LexicalTokenType.EndSubExpression:
                        // ')'
                        // Pop until we hit '('
                        var foundParenthesis = false;
                        while (true)
                        {
                            var opObject = operatorStack.TryPop();
                            var op = opObject?.OperatorSymbol;
                            if (op == null) break; // Stop if at end of stack
                            if (op == "(")
                            {
                                foundParenthesis = true;
                                break;
                            }
                            outputQueue.Enqueue(new ParsedOperatorToken(opObject));
                        }
                        if (!foundParenthesis)
                        {
                            RaiseParserError(token, "Mismatched parenthesis");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // Check the operator stack:
            if (operatorStack.Count > 0)
            {
                var op = operatorStack.Peek();
                if (op.OperatorSymbol == "(" || op.OperatorSymbol == ")")
                {
                    // Mismatched parenthesis!
                    RaiseParserError(null, "Mismatched parenthesis");
                    return null;
                }
                while ((op = operatorStack.TryPop()) != null)
                {
                    outputQueue.Enqueue(new ParsedOperatorToken(op));
                }
            }
            return outputQueue.ToArray();
        }

        /// <summary>
        /// Handles a comma token
        /// </summary>
        /// <param name="holderStruct"></param>
        /// <param name="token"></param>
        private void HandleComma(RpnHolderStruct holderStruct, LexicalToken token)
        {
            // Pop of operators and put on the output queue until we hit a '('. If not encountered, bad ',' or 'Mismatched parenthesis'
            Operator op;
            while ((op = holderStruct.OperatorStack.TryPeek()) != null)
            {
                if (op.OperatorSymbol == "(")
                {
                    // Done!
                    return;
                }
                // Pop it now:
                holderStruct.OperatorStack.Pop(); // Already have the operator off the stack
                holderStruct.OutputQueue.Enqueue(new ParsedOperatorToken(holderStruct.OperatorStack.Pop()));
            }
            // Getting here means we popped the entire stack without finding a ','!
            RaiseParserError(token, $"Bad comma: Mismatched parenthesis");
        }

        /// <summary>
        /// Handles a operator token
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        private void HandleOperator(RpnHolderStruct holder, LexicalToken token)
        {
            ValidateTokenHasValue(token);
            // Is it a late bind
            Operator @operator;
            if (!_operatorsDictionary.TryGetValue(token.Value.ToLower(), out @operator))
            {
                RaiseParserError(token, $"Unrecognised operator '{ token.Value }'");
                return;
            }

            var lastOperator = holder.OperatorStack.TryPeek();
            if (lastOperator != null && lastOperator.OperatorSymbol != "(")
            {
                var op = @operator;

                // Check funcs:
                var checkFunc = (op.Associativity == OperatorAssociativity.Left) ?
                     new Func<Operator, bool>((o) => o.Precedence <= lastOperator.Precedence) :
                     new Func<Operator, bool>(((o) => o.Precedence < lastOperator.Precedence));

                if (checkFunc(op))
                {
                    // Pop off and put in the output queue:
                    holder.OutputQueue.Enqueue(new ParsedOperatorToken(holder.OperatorStack.Pop()));
                }
            }

            holder.OperatorStack.Push(@operator);
        }

        /// <summary>
        /// Handles a WORD token
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        private void HandleWord(RpnHolderStruct holder, LexicalToken token)
        {
            // Check: is it length 1?
            var value = token.Value.ToUpper();

            if (value.Length == 1 && char.IsLetter(value[0]))
            {
                // It is a var. name
                holder.OutputQueue.Enqueue(new ParsedVariableToken(value));
                return;
            }
            // Otherwise: Constant or mathematical function:

            // Is it a predefined constant?
            double constValue;
            Operator op;
            if (_constantsDictionary.TryGetValue(value, out constValue))
            {
                holder.OutputQueue.Enqueue(new ParsedConstantToken(value, constValue));
                return;
            }
            if (_operatorsDictionary.TryGetValue(value.ToLower(), out op)) // Is it a function 'operator'
            {
                // Hand off to the operator handler
                HandleOperator(holder, token);
                return;
            }
            // Neither, fail
            RaiseParserError(token, $"'{value}' is not a valid constant, function or variable name");
        }

        /// <summary>
        /// Raises a parser error
        /// </summary>
        /// <param name="token"></param>
        /// <param name="errMsg"></param>
        private void RaiseParserError(LexicalToken token, string errMsg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies that the given token has a non-empty/null value
        /// </summary>
        /// <param name="token"></param>
        private void ValidateTokenHasValue(LexicalToken token)
        {
            if (string.IsNullOrEmpty(token.Value)) RaiseParserError(token, $"Expected a value for the token '{ token.GetTypeAsName() }'");
        }

        /// <summary>
        /// Internal struct for passing around the output queue and operator stack easily
        /// </summary>
        private struct RpnHolderStruct
        {
            public RpnHolderStruct(Queue<ParsedToken> outputQueue, Stack<Operator> operatorStack)
            {
                OutputQueue = outputQueue;
                OperatorStack = operatorStack;
            }

            public Stack<Operator> OperatorStack { get; private set; }
            public Queue<ParsedToken> OutputQueue { get; private set; }
        }
    }
}
