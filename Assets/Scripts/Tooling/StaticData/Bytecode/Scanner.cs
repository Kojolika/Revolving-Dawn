using System.Collections.Generic;
using Tooling.Logging;
using Utils.Extensions;

namespace Tooling.StaticData.Bytecode
{
    public interface IErrorReport
    {
        /// <summary>
        /// Implementers can handle how errors are dealt with
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="columnNumber"> the index of the string where the start of the error occured s</param>
        /// <param name="length"> the length of the error </param>
        void Report(string message, int columnNumber, int length);
    }

    /// <summary>
    /// Reads a string and returns a list of Tokens.
    /// This is the lexical analysis part of a compiler.
    /// </summary>
    public static class Scanner
    {
        /// <param name="source"> The string to create tokens from</param>
        /// <param name="tokens"> The tokens created </param>
        /// <param name="errorReport"> where the errors are passed to </param>
        /// <returns> true if no errors have occurred </returns>
        public static bool Scan(string source, out List<Token> tokens, IErrorReport errorReport = null)
        {
            tokens = new List<Token>();
            if (source.IsNullOrEmpty())
            {
                return true;
            }

            int sourceLength = source.Length;
            int currentIndex = 0;
            bool hasError = false;

            while (!IsAtEnd())
            {
                char currentChar = source[currentIndex];
                switch (currentChar)
                {
                    case '(':
                        tokens.Add(new Token(Token.Type.LeftParen, currentIndex, 1, currentChar.ToString()));
                        break;
                    case ')':
                        tokens.Add(new Token(Token.Type.RightParen, currentIndex, 1, currentChar.ToString()));
                        break;
                    case '}':
                        tokens.Add(new Token(Token.Type.RightBrace, currentIndex, 1, currentChar.ToString()));
                        break;
                    case ',':
                        tokens.Add(new Token(Token.Type.Comma, currentIndex, 1, currentChar.ToString()));
                        break;
                    case '.':
                        tokens.Add(new Token(Token.Type.Dot, currentIndex, 1, currentChar.ToString()));
                        break;
                    case '-':
                        tokens.Add(new Token(Token.Type.Minus, currentIndex, 1, currentChar.ToString()));
                        break;
                    case '+':
                        tokens.Add(new Token(Token.Type.Plus, currentIndex, 1, currentChar.ToString()));
                        break;
                    case ';':
                        tokens.Add(new Token(Token.Type.Semicolon, currentIndex, 1, currentChar.ToString()));
                        break;
                    case '/':
                        tokens.Add(new Token(Token.Type.Slash, currentIndex, 1, currentChar.ToString()));
                        break;
                    case '*':
                        tokens.Add(new Token(Token.Type.Star, currentIndex, 1, currentChar.ToString()));
                        break;

                    case '!':
                    {
                        if (TryPeek(out var next) && next == '=')
                        {
                            tokens.Add(new Token(Token.Type.BangEqual, currentIndex, 2, currentChar.ToString()));
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Bang, currentIndex, 1, currentChar.ToString()));
                        }

                        break;
                    }

                    case '=':
                    {
                        if (TryPeek(out var next) && next == '=')
                        {
                            tokens.Add(new Token(Token.Type.EqualEqual, currentIndex, 2, currentChar.ToString()));
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Equal, currentIndex, 1, currentChar.ToString()));
                        }

                        break;
                    }


                    case '<':
                    {
                        if (TryPeek(out var next) && next == '=')
                        {
                            tokens.Add(new Token(Token.Type.LessEqual, currentIndex, 2, currentChar.ToString()));
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Less, currentIndex, 1, currentChar.ToString()));
                        }

                        break;
                    }

                    case '>':
                    {
                        if (TryPeek(out var next) && next == '=')
                        {
                            tokens.Add(new Token(Token.Type.GreaterEqual, currentIndex, 2, currentChar.ToString()));
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Greater, currentIndex, 1, currentChar.ToString()));
                        }

                        break;
                    }

                    case '{':
                    {
                        if (!TryPeek(out var next) || !IsAlpha(next))
                        {
                            tokens.Add(new Token(Token.Type.LeftBrace, currentIndex, 1, currentChar.ToString()));
                        }
                        else
                        {
                            int lexemeLength = 1;
                            int startIndex = currentIndex;
                            while (TryPeek(out next) && IsAlphaNumeric(next))
                            {
                                lexemeLength++;
                                currentIndex++;
                            }

                            currentIndex++;
                            lexemeLength++;

                            if (IsAtEnd() || source[currentIndex] != '}')
                            {
                                errorReport?.Report("Invalid variable, missing end brace '}'" + currentIndex, startIndex, lexemeLength);
                                tokens.Add(new Token(Token.Type.Error, startIndex, lexemeLength, source.Substring(startIndex, lexemeLength)));
                            }
                            else
                            {
                                tokens.Add(new Token(Token.Type.ByteVar, startIndex, lexemeLength, source.Substring(startIndex, lexemeLength)));
                            }
                        }

                        break;
                    }

                    case var c when IsAlpha(c):
                    {
                        int lexemeLength = 1;
                        int startIndex = currentIndex;
                        while (!IsAtEnd() && TryPeek(out var next) && IsAlphaNumeric(next))
                        {
                            lexemeLength++;
                            currentIndex++;
                        }

                        string lexeme = source.Substring(startIndex, lexemeLength);
                        switch (lexeme)
                        {
                            case "and":
                                tokens.Add(new Token(Token.Type.And, startIndex, lexemeLength, lexeme));
                                break;
                            case "or":
                                tokens.Add(new Token(Token.Type.Or, startIndex, lexemeLength, lexeme));
                                break;
                            case "if":
                                tokens.Add(new Token(Token.Type.If, startIndex, lexemeLength, lexeme));
                                break;
                            case "else":
                                tokens.Add(new Token(Token.Type.Else, startIndex, lexemeLength, lexeme));
                                break;
                            case "true":
                                tokens.Add(new Token(Token.Type.True, startIndex, lexemeLength, lexeme));
                                break;
                            case "false":
                                tokens.Add(new Token(Token.Type.False, startIndex, lexemeLength, lexeme));
                                break;
                            case "null":
                                tokens.Add(new Token(Token.Type.Null, startIndex, lexemeLength, lexeme));
                                break;
                            case "while":
                                tokens.Add(new Token(Token.Type.While, startIndex, lexemeLength, lexeme));
                                break;
                            case "for":
                                tokens.Add(new Token(Token.Type.For, startIndex, lexemeLength, lexeme));
                                break;
                            case "func":
                                tokens.Add(new Token(Token.Type.Function, startIndex, lexemeLength, lexeme));
                                break;
                            case "print":
                                tokens.Add(new Token(Token.Type.Print, startIndex, lexemeLength, lexeme));
                                break;
                            case "return":
                                tokens.Add(new Token(Token.Type.Return, startIndex, lexemeLength, lexeme));
                                break;
                            case "var":
                                tokens.Add(new Token(Token.Type.Var, startIndex, lexemeLength, lexeme));
                                break;
                            default:
                                tokens.Add(new Token(Token.Type.Identifier, startIndex, lexemeLength, lexeme));
                                break;
                        }

                        break;
                    }

                    case '"':
                    {
                        int lexemeLength = 1;
                        int startIndex = currentIndex;
                        while (TryPeek(out var next) && next != '"')
                        {
                            lexemeLength++;
                            currentIndex++;
                        }

                        currentIndex++;

                        if (IsAtEnd() || source[currentIndex] != '"')
                        {
                            hasError = true;
                            errorReport?.Report("Unterminated string.", startIndex, lexemeLength);
                            tokens.Add(new Token(Token.Type.Error, startIndex, lexemeLength, source.Substring(startIndex, lexemeLength)));
                        }
                        else
                        {
                            // trim the quotes
                            tokens.Add(new Token(Token.Type.String, startIndex, lexemeLength, source.Substring(startIndex + 1, lexemeLength - 1)));
                        }


                        break;
                    }


                    case var c when char.IsDigit(c):
                    {
                        int lexemeLength = 1;
                        int startIndex = currentIndex;
                        while (TryPeek(out var next) && (next == '.' || char.IsDigit(next)))
                        {
                            lexemeLength++;
                            currentIndex++;
                        }

                        if (!IsAtEnd() && source[currentIndex] == '.')
                        {
                            errorReport?.Report("Invalid number, cannot end a number with a '.'", startIndex, lexemeLength);
                            tokens.Add(new Token(Token.Type.Error, startIndex, lexemeLength, source.Substring(startIndex, lexemeLength)));
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Number, startIndex, lexemeLength, source.Substring(startIndex, lexemeLength)));
                        }

                        break;
                    }

                    case ' ':
                    case '\r':
                    case '\t':
                    case '\n':
                        // Ignore whitespace
                        break;

                    default:
                        hasError = true;
                        errorReport?.Report($"Unrecognized character {source[currentIndex]}", currentIndex, 1);
                        tokens.Add(new Token(Token.Type.Error, currentIndex, 1, source.Substring(currentIndex, 1)));
                        break;
                }

                currentIndex++;
            }

            tokens.Add(new Token(Token.Type.Eof, currentIndex, 0, string.Empty));

            return hasError;

            bool IsAtEnd() => currentIndex >= sourceLength;

            bool TryPeek(out char next)
            {
                next = default;
                if (currentIndex + 1 >= sourceLength)
                {
                    return false;
                }

                next = source[currentIndex + 1];
                return true;
            }

            bool IsAlpha(char c)
            {
                return char.IsLetter(c) || c == '_';
            }

            bool IsAlphaNumeric(char c)
            {
                return char.IsLetterOrDigit(c) || c == '_';
            }
        }
    }

    public class Token
    {
        public readonly Type TokenType;

        /// <summary>
        /// The offset from the start of the input
        /// </summary>
        public readonly int Start;

        /// <summary>
        /// The length of the token
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// The raw token
        /// </summary>
        public readonly string Lexeme;

        public Token(Type tokenType, int start, int length, string lexeme)
        {
            TokenType = tokenType;
            Start = start;
            Length = length;
            Lexeme = lexeme;
        }

        public enum Type
        {
            // Single-character tokens.
            LeftParen,
            RightParen,
            LeftBrace,
            RightBrace,
            Comma,
            Dot,
            Minus,
            Plus,
            Semicolon,
            Slash,
            Star,

            // One or two character tokens.
            Bang,
            BangEqual,
            Equal,
            EqualEqual,
            Greater,
            GreaterEqual,
            Less,
            LessEqual,

            // Literals.
            Identifier,
            String,
            Number,

            // Custom bytecode variable
            ByteVar,

            // Keywords.
            And,
            Or,
            If,
            Else,
            True,
            False,
            Null,
            While,
            For,
            Function,
            Print,
            Return,
            Super,
            This,
            Var,
            Class,
            Eof,
            Error
        }
    }

    public static class Interpreter
    {
        public static bool Interpret(List<Token> tokens, out List<byte> bytecode, IErrorReport errorReport = null)
        {
            bytecode = new List<byte>();
            if (tokens.IsNullOrEmpty())
            {
                return true;
            }

            var parser = new Parser();
            Advance(parser, tokens, errorReport);

            EmitReturn(bytecode);
            return !parser.HadError;
        }

        private static void Advance(Parser parser, List<Token> tokens, IErrorReport errorReport = null)
        {
            parser.Previous = parser.Current;

            int i = 0;
            while (true)
            {
                parser.Current = tokens[i];
                if (parser.Current.TokenType != Token.Type.Error)
                {
                    break;
                }

                ErrorAtCurrent(parser, $"Error at: {parser.Current.Start}:{parser.Current.Length}", errorReport);
            }
        }

        private static void Consume(Token.Type tokenType, Parser parser, List<Token> tokens, string message, IErrorReport errorReport = null)
        {
            if (parser.Current.TokenType == tokenType)
            {
                Advance(parser, tokens, errorReport);
                return;
            }

            ErrorAtCurrent(parser, message, errorReport);
        }

        private static void EmitByte(byte value, List<byte> bytes)
        {
            bytes.Add(value);
        }

        private static void EmitReturn(List<byte> bytes)
        {
            bytes.Add((byte)ByteCode.Return);
        }

        private static void ErrorAtCurrent(Parser parser, string message, IErrorReport errorReport = null)
        {
            if (parser.PanicMode)
            {
                return;
            }

            parser.HadError = true;
            parser.PanicMode = true;
            errorReport?.Report(message, parser.Current.Start, parser.Current.Length);
        }

        private class Parser
        {
            public Token Previous;
            public Token Current;
            public bool HadError;
            public bool PanicMode;
        }
    }

    public enum ByteCode
    {
        Return,
    }
}