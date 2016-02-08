using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Alistair.Tudor.MathsFormulaParser.Exceptions;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Helpers.Extensions;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.LexicalAnalysis;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers;
using Alistair.Tudor.MathsFormulaParser.Internal.Parsers.ParserHelpers.Tokens;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Functions;
using Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Operators;
using Alistair.Tudor.Utility.Extensions;
using Operator = Alistair.Tudor.MathsFormulaParser.Internal.Symbols.Operators.Operator;

namespace Alistair.Tudor.MathsFormulaParser.Internal.Parsers
{
    /// <summary>
    /// Parses the lexical tokens to ParsedTokens
    /// </summary>
    internal class Parser
    {
        /// <summary>
        /// Regex for checking variable names
        /// </summary>
        private static readonly Regex VariableWordMatchRegex = new Regex("^[A-Z]{1}[A-Z0-9_]*$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        /// <summary>
        /// Dictionary of constants
        /// </summary>
        private readonly Dictionary<string, Constant> _constantsDictionary = new Dictionary<string, Constant>();

        /// <summary>
        /// Original input formula
        /// </summary>
        private readonly string _formula;

        /// <summary>
        /// Dictionary of registered functions and operators
        /// </summary>
        private readonly Dictionary<string, StandardFunction> _functionsDictionary = new Dictionary<string, StandardFunction>();

        /// <summary>
        /// Dictionary of registered operators
        /// </summary>
        private readonly Dictionary<string, Operator> _operatorsDictionary = new Dictionary<string, Operator>()
        {
             // Pseudo Function - not used in output!
            {SpecialConstants.SubExpressionStart, new Operator(SpecialConstants.SubExpressionStart, int.MaxValue, OperatorAssociativity.Left, i => { throw new InvalidOperationException($"Internal Error: Cannot Invoke the '{SpecialConstants.SubExpressionStart}' pseudo Function!"); }, 0)},
        };
        /// <summary>
        /// Input list of lexical tokens
        /// </summary>
        private readonly LexicalToken[] _tokens;

        /// <summary>
        /// Dictionary of registered unary operators
        /// </summary>
        private readonly Dictionary<string, Operator> _unaryOperatorsDictionary = new Dictionary<string, Operator>();
        /// <summary>
        /// List of used tokens
        /// </summary>
        private readonly List<LexicalToken> _usedTokenList = new List<LexicalToken>();

        /// <summary>
        /// Current parser state
        /// </summary>
        private ParserState _currentParserState = ParserState.Normal;
        /// <summary>
        /// Output list of parsed tokens
        /// </summary>
        private ParsedToken[] _rpnTokens;

        public Parser(string formula, LexicalToken[] tokens, IEnumerable<Operator> operators, IEnumerable<StandardFunction> customFunctions,
                            IEnumerable<Constant> customConstants = null)
        {
            _formula = formula;
            _tokens = tokens;

            if (customConstants != null)
            {
                foreach (var map in customConstants)
                {
                    _constantsDictionary.AddOrUpdateValue(map.Name, map);
                }
            }

            if (operators != null)
            {
                foreach (var @operator in operators)
                {
                    if (@operator.IsUnaryOperator)
                    {
                        _unaryOperatorsDictionary.AddOrUpdateValue(@operator.OperatorSymbol, @operator);
                    }
                    else
                    {
                        _operatorsDictionary.AddOrUpdateValue(@operator.OperatorSymbol, @operator);
                    }
                }
            }

            if (customFunctions != null)
            {
                foreach (var func in customFunctions)
                {
                    _functionsDictionary.AddOrUpdateValue(func.FunctionName, func);
                }
            }
        }

        /// <summary>
        /// Last lexical token. Whitespace tokens are not returned
        /// </summary>
        private LexicalToken LastLexicalToken => GetUsedLexicalTokens().FirstOrDefault();

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
                RaiseParserError(null, $"Unexpected parser state: Expected '{ Enum.GetName(typeof(ParserState), expectedState)  }', but found '{ Enum.GetName(typeof(ParserState), _currentParserState) }'", true);
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
            var operatorStack = new Stack<FunctionHolder>();
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

                HandleLexicalToken(holderStruct, reader, token);
            }
            // Check our end state:

            if (_currentParserState != ParserState.Normal)
            {
                switch (_currentParserState)
                {
                    case ParserState.InSubScript:
                        // Error - mismatching subscripts!
                        RaiseParserError(null, "Mismatched subscripts", false);
                        return null;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // Check the Function stack:
            if (operatorStack.Count > 0)
            {
                FunctionHolder op;
                while ((op = operatorStack.TryPop()) != null)
                {
                    if (op.Function.FunctionName == SpecialConstants.SubExpressionStart || op.Function.FunctionName == SpecialConstants.SubExpressionEnd)
                    {
                        // Mismatched parenthesis!
                        RaiseParserError(op.Token, "Mismatched parenthesis", false);
                        return null;
                    }
                    outputQueue.Enqueue(new ParsedFunctionToken(op.Function, GetTokenPosition(op.Token)));
                }
            }
            return outputQueue.ToArray();
        }

        /// <summary>
        /// Gets the current position of the given lexical token
        /// </summary>
        /// <returns></returns>
        private long GetTokenPosition(LexicalToken token)
        {
            return token?.CharacterPosition ?? -1;
        }

        /// <summary>
        /// Gets an IEnumerable of "used" tokens
        /// </summary>
        /// <returns></returns>
        private IEnumerable<LexicalToken> GetUsedLexicalTokens()
        {
            for (var i = _usedTokenList.Count - 1; i >= 0; i--)
            {
                var token = _usedTokenList[i];
                yield return token;
            }
        }
        /// <summary>
        /// Handles a comma token
        /// </summary>
        /// <param name="holderStruct"></param>
        /// <param name="token"></param>
        private void HandleComma(RpnHolderStruct holderStruct, LexicalToken token)
        {
            // Pop off operators and put on the output queue until we hit a '('. If not encountered, bad ',' or 'Mismatched parenthesis'
            FunctionHolder func;
            while ((func = holderStruct.OperatorStack.TryPeek()) != null)
            {
                if (func.Function.FunctionName == SpecialConstants.SubExpressionStart)
                {
                    // Done!
                    return;
                }
                // Pop it now:
                holderStruct.OperatorStack.Pop(); // Already have the Function via peek. Take it off the stack
                holderStruct.OutputQueue.Enqueue(new ParsedFunctionToken(func.Function, GetTokenPosition(func.Token)));
            }
            // Getting here means we popped the entire stack without finding a ','!
            RaiseParserError(token, $"Bad comma: Mismatched parenthesis", false);
        }
        /// <summary>
        /// Handles the end of a sub expression (')')
        /// </summary>
        /// <param name="holderStruct"></param>
        /// <param name="token"></param>
        private void HandleEndSubExpression(RpnHolderStruct holderStruct, LexicalToken token)
        {
            var operatorStack = holderStruct.OperatorStack;
            var outputQueue = holderStruct.OutputQueue;

            // Pop until we hit '('
            var foundParenthesis = false;
            while (true)
            {
                var opObject = operatorStack.TryPop();
                var op = opObject?.Function?.FunctionName;
                if (op == null) break; // Stop if at end of stack
                if (op == SpecialConstants.SubExpressionStart)
                {
                    foundParenthesis = true;
                    break;
                }
                outputQueue.Enqueue(new ParsedFunctionToken(opObject.Function, GetTokenPosition(token)));

            }
            if (!foundParenthesis)
            {
                RaiseParserError(token, "Mismatched parenthesis", false);
            }
        }

        /// <summary>
        /// Handles a bit index operation (e.g. 5[1])
        /// This is called at the end of the Sub Script
        /// </summary>
        /// <param name="holderStruct"></param>
        /// <param name="reader"></param>
        /// <param name="token"></param>
        private void HandleEndSubScript(RpnHolderStruct holderStruct, LinearTokenReader<LexicalToken> reader, LexicalToken token)
        {
            ChangeParserStart(ParserState.Normal, ParserState.InSubScript); // Change the state back
            // Also push a ')' to end the subexpression:
            HandleLexicalToken(holderStruct, reader, new LexicalToken(LexicalTokenType.EndSubExpression, ")", token.CharacterPosition));
        }

        /// <summary>
        /// Handles a Function call
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="function"></param>
        /// <param name="token"></param>
        private void HandleFunctionCall(RpnHolderStruct holder, FormulaFunction function, LexicalToken token)
        {
            holder.OperatorStack.Push(new FunctionHolder(function, token));
        }

        /// <summary>
        /// Processes a lexical token
        /// </summary>
        /// <param name="holderStruct"></param>
        /// <param name="reader"></param>
        /// <param name="token"></param>
        private void HandleLexicalToken(RpnHolderStruct holderStruct, LinearTokenReader<LexicalToken> reader, LexicalToken token)
        {
            switch (token.TokenType)
            {
                case LexicalTokenType.Space:
                    return; // Ignore
                case LexicalTokenType.Number:
                    HandleNumber(holderStruct, token);
                    break;
                case LexicalTokenType.Word: // Variable or constant or function
                    HandleWord(holderStruct, token);
                    break;
                case LexicalTokenType.Comma:
                    HandleComma(holderStruct, token);
                    break;
                case LexicalTokenType.Operator:
                    HandleOperator(holderStruct, reader, token);
                    break;
                case LexicalTokenType.StartSubScript:
                    HandleStartSubScript(holderStruct, reader, token);
                    break;
                case LexicalTokenType.EndSubScript:
                    HandleEndSubScript(holderStruct, reader, token);
                    break;
                case LexicalTokenType.StartSubExpression:
                    // '('
                    HandleFunctionCall(holderStruct, _operatorsDictionary[SpecialConstants.SubExpressionStart], token); // Special op - guaranteed to be there!
                    break;
                case LexicalTokenType.EndSubExpression:
                    // ')'
                    HandleEndSubExpression(holderStruct, token);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _usedTokenList.Add(token);
        }

        /// <summary>
        /// Handles a NUMBER token
        /// </summary>
        /// <param name="holderStruct"></param>
        /// <param name="token"></param>
        private void HandleNumber(RpnHolderStruct holderStruct, LexicalToken token)
        {
            ValidateTokenHasValue(token);
            ValidateCorrectPreviousTokenForOperand(token);

            // Add to the output queue:
            holderStruct.OutputQueue.Enqueue(new ParsedNumberToken(double.Parse(token.Value, CultureInfo.InvariantCulture), GetTokenPosition(token)));
        }

        /// <summary>
        /// Handles a operator token
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="reader"></param>
        /// <param name="token"></param>
        private void HandleOperator(RpnHolderStruct holder, LinearTokenReader<LexicalToken> reader, LexicalToken token)
        {
            ValidateTokenHasValue(token);

            // CHECK: Is it in a unary operator position?
            var isUnary = IsInUnaryOperatorPosition(holder, reader, token);
            var dic = isUnary ? _unaryOperatorsDictionary : _operatorsDictionary;

            Operator @operator;
            if (!dic.TryGetValue(token.Value.ToLower(), out @operator))
            {
                RaiseParserError(token, $"Unrecognised { (isUnary ? "Unary" : "") } operator '{token.Value}'", false);
                return;
            }

            var lastOperator = holder.OperatorStack.TryPeek()?.Function as Operator;
            if (lastOperator != null && lastOperator.FunctionName != SpecialConstants.SubExpressionStart)
            {
                if (OperatorPrecedenceCheck(@operator, lastOperator))
                {
                    // Pop off and put in the output queue:
                    holder.OutputQueue.Enqueue(new ParsedFunctionToken(holder.OperatorStack.Pop().Function, GetTokenPosition(null)));
                }
            }

            HandleFunctionCall(holder, @operator, token);
        }

        /// <summary>
        /// Handles a bit index operation (e.g. 5[1])
        /// This is called at the start of the Sub Script
        /// </summary>
        /// <param name="holderStruct"></param>
        /// <param name="reader"></param>
        /// <param name="token"></param>
        private void HandleStartSubScript(RpnHolderStruct holderStruct, LinearTokenReader<LexicalToken> reader, LexicalToken token)
        {
            // This is == to the function 'GetBit(number, bitPos)'
            // We have a pseudo operator '[' that handles this operation.
            // Therefore: Just forward as an operator and change the state
            ChangeParserStart(ParserState.InSubScript, ParserState.Normal);
            HandleLexicalToken(holderStruct, reader, new LexicalToken(LexicalTokenType.Operator, SpecialConstants.GetBitOperatorSymbol, token.CharacterPosition));
            // Also push a '(' to ensure a subexpression:
            HandleLexicalToken(holderStruct, reader, new LexicalToken(LexicalTokenType.StartSubExpression, SpecialConstants.SubExpressionStart, token.CharacterPosition));
        }
        /// <summary>
        /// Handles a WORD token - this could be constant, variable or 
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="token"></param>
        private void HandleWord(RpnHolderStruct holder, LexicalToken token)
        {
            ValidateTokenHasValue(token);
            ValidateCorrectPreviousTokenForOperand(token);

            var value = token.Value.ToUpper();

            // Check: Constant or mathematical function?

            // Is it a predefined constant?
            Constant constant;
            if (_constantsDictionary.TryGetValue(value, out constant))
            {
                holder.OutputQueue.Enqueue(new ParsedConstantToken(value, constant.Value, GetTokenPosition(token)));
                return;
            }

            StandardFunction func;
            if (_functionsDictionary.TryGetValue(value.ToLower(), out func)) // Is it a Function?
            {
                // Hand off to the Function handler
                HandleFunctionCall(holder, func, token);
                return;
            }

            // Last Check: Is it a valid variable form?
            if (IsValidWordForVariable(value))
            {
                // It is a var. name
                holder.OutputQueue.Enqueue(new ParsedVariableToken(value, GetTokenPosition(token)));
                return;
            }

            // Nothing, fail
            RaiseParserError(token, $"'{value}' is not a valid constant, function or variable name", false);
        }

        /// <summary>
        /// Gets whether the current operator token is in a unary operator position
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="reader"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool IsInUnaryOperatorPosition(RpnHolderStruct holder, LinearTokenReader<LexicalToken> reader, LexicalToken token)
        {
            // Previous token can only be an operator, expression or null token:
            // 5 + -5 = 0
            // A-5 + B - 5
            if (LastLexicalToken != null)
            {
                switch (LastLexicalToken.TokenType)
                {
                    case LexicalTokenType.StartSubExpression:
                    case LexicalTokenType.Operator:
                    case LexicalTokenType.StartSubScript:
                    case LexicalTokenType.Comma:
                        break;
                    default:
                        return false;
                }
            }

            // Peek the next character: 
            // It can only be WORD, NUMBER or '(' OPERATOR
            // E.g.: -A & -9 & -(A + 8)
            var peekedItem = reader.TryPeek(null);
            if (peekedItem == null) return false; // No more items
            switch (peekedItem.TokenType)
            {
                case LexicalTokenType.Number:
                case LexicalTokenType.Word:
                case LexicalTokenType.StartSubExpression:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether the given string is a valid variable name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool IsValidWordForVariable(string value)
        {
            return VariableWordMatchRegex.IsMatch(value);
        }

        private bool OperandPreviousTokenCheck(LexicalToken token)
        {
            var t = token.TokenType;
            switch (t)
            {
                //case LexicalTokenType.StartSubExpression:
                //case LexicalTokenType.StartSubScript:
                case LexicalTokenType.EndSubExpression:
                case LexicalTokenType.EndSubScript:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Operator Precedence Check 
        /// </summary>
        /// <param name="currentFunction"></param>
        /// <param name="lastFunction"></param>
        /// <returns></returns>
        private bool OperatorPrecedenceCheck(Operator currentFunction, Operator lastFunction)
        {
            var currentPrecedence = currentFunction.Precedence;
            var lastPrecedence = lastFunction.Precedence;

            switch (currentFunction.Associativity)
            {
                case OperatorAssociativity.Left:
                    return currentPrecedence <= lastPrecedence;
                case OperatorAssociativity.Right:
                    return currentPrecedence < lastPrecedence;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Raises a parser error
        /// </summary>
        /// <param name="token"></param>
        /// <param name="errMsg"></param>
        /// <param name="isInternalError"></param>
        private void RaiseParserError(LexicalToken token, string errMsg, bool isInternalError)
        {
            var prefix = isInternalError ? "Internal Parser Error: " : "";
            throw new FormulaParseException($"{prefix}{errMsg}", GetTokenPosition(token));
        }

        /// <summary>
        /// Verifies that the previous token is valid for an operand
        /// </summary>
        /// <param name="token"></param>
        private void ValidateCorrectPreviousTokenForOperand(LexicalToken token)
        {
            var last = GetUsedLexicalTokens()
                .FirstOrDefault(OperandPreviousTokenCheck);

            if (last == null) return; // No previous token, so OK
            switch (last.TokenType)
            {
                // Cannot have consecutive WORDs or NUMBERS!
                case LexicalTokenType.Word:
                case LexicalTokenType.Number:
                    RaiseParserError(token, $"Unexpected {token.GetTypeAsName()}. An operator or function call was expected.", false);
                    return;
            }
        }
        /// <summary>
        /// Verifies that the given token has a non-empty/null value
        /// </summary>
        /// <param name="token"></param>
        private void ValidateTokenHasValue(LexicalToken token)
        {
            if (string.IsNullOrEmpty(token.Value)) RaiseParserError(token, $"Expected a value for the token '{token.GetTypeAsName()}'", true);
        }

        /// <summary>
        /// Holder for function stack
        /// </summary>
        private class FunctionHolder
        {
            public FunctionHolder(FormulaFunction function, LexicalToken token)
            {
                Function = function;
                Token = token;
            }

            public FormulaFunction Function { get; }
            public LexicalToken Token { get; }

            public override string ToString()
            {
                return Function?.ToString() ?? "";
            }
        }

        /// <summary>
        /// Internal struct for passing around the output queue and Function stack easily
        /// </summary>
        private class RpnHolderStruct
        {
            public RpnHolderStruct(Queue<ParsedToken> outputQueue, Stack<FunctionHolder> operatorStack)
            {
                OutputQueue = outputQueue;
                OperatorStack = operatorStack;
            }

            public Stack<FunctionHolder> OperatorStack { get; private set; }
            public Queue<ParsedToken> OutputQueue { get; private set; }
        }
    }
}
