using System.Collections.Generic;
using System.Linq;
using Tooling.Logging;
using Utils.Extensions;

namespace Tooling.StaticData.Data.Bytecode
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
        public static bool Scan(string source, out List<Token> tokens, ILogger logger = null, IErrorReport errorReport = null)
        {
            tokens = new List<Token>();
            if (source.IsNullOrEmpty())
            {
                return true;
            }

            int sourceLength = source.Length;
            int currentIndex = 0;
            int line = 0;
            bool hasError = false;

            logger?.Log(LogLevel.Info, "==== Scanning ====");
            while (!IsAtEnd())
            {
                char currentChar = source[currentIndex];
                switch (currentChar)
                {
                    case '(':
                        tokens.Add(new Token(Token.Type.LeftParen, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;
                    case ')':
                        tokens.Add(new Token(Token.Type.RightParen, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;
                    case '}':
                        tokens.Add(new Token(Token.Type.RightBrace, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;
                    case ',':
                        tokens.Add(new Token(Token.Type.Comma, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;
                    case '.':
                        tokens.Add(new Token(Token.Type.Dot, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;
                    case '-':
                        tokens.Add(new Token(Token.Type.Minus, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;
                    case '+':
                        tokens.Add(new Token(Token.Type.Plus, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;
                    case ';':
                        tokens.Add(new Token(Token.Type.Semicolon, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;
                    case '/':
                        tokens.Add(new Token(Token.Type.Slash, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;
                    case '*':
                        tokens.Add(new Token(Token.Type.Star, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        break;

                    case '!':
                    {
                        if (TryPeek(out var next) && next == '=')
                        {
                            tokens.Add(new Token(Token.Type.BangEqual, currentIndex, 2, currentChar.ToString(), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Bang, currentIndex, 1, currentChar.ToString(), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        }

                        break;
                    }

                    case '=':
                    {
                        if (TryPeek(out var next) && next == '=')
                        {
                            tokens.Add(new Token(Token.Type.EqualEqual, currentIndex, 2, currentChar.ToString(), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Equal, currentIndex, 1, currentChar.ToString(), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        }

                        break;
                    }


                    case '<':
                    {
                        if (TryPeek(out var next) && next == '=')
                        {
                            tokens.Add(new Token(Token.Type.LessEqual, currentIndex, 2, currentChar.ToString(), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Less, currentIndex, 1, currentChar.ToString(), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        }

                        break;
                    }

                    case '>':
                    {
                        if (TryPeek(out var next) && next == '=')
                        {
                            tokens.Add(new Token(Token.Type.GreaterEqual, currentIndex, 2, currentChar.ToString(), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Greater, currentIndex, 1, currentChar.ToString(), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        }

                        break;
                    }

                    case '{':
                    {
                        tokens.Add(new Token(Token.Type.LeftBrace, currentIndex, 1, currentChar.ToString(), line));
                        logger?.Log(LogLevel.Info, tokens.Last().ToString());
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
                                tokens.Add(new Token(Token.Type.And, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "or":
                                tokens.Add(new Token(Token.Type.Or, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "if":
                                tokens.Add(new Token(Token.Type.If, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "else":
                                tokens.Add(new Token(Token.Type.Else, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "true":
                                tokens.Add(new Token(Token.Type.True, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "false":
                                tokens.Add(new Token(Token.Type.False, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "null":
                                tokens.Add(new Token(Token.Type.Null, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "while":
                                tokens.Add(new Token(Token.Type.While, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "for":
                                tokens.Add(new Token(Token.Type.For, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "func":
                                tokens.Add(new Token(Token.Type.Function, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "print":
                                tokens.Add(new Token(Token.Type.Print, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "return":
                                tokens.Add(new Token(Token.Type.Return, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            case "var":
                                tokens.Add(new Token(Token.Type.Var, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
                                break;
                            default:
                                tokens.Add(new Token(Token.Type.Identifier, startIndex, lexemeLength, lexeme, line));
                                logger?.Log(LogLevel.Info, tokens.Last().ToString());
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
                            tokens.Add(new Token(Token.Type.Error, startIndex, lexemeLength, source.Substring(startIndex, lexemeLength), line));
                            logger?.Log(LogLevel.Error, $"{tokens.Last()} ERROR: Unterminated string.");
                        }
                        else
                        {
                            // trim the quotes
                            tokens.Add(new Token(Token.Type.String, startIndex, lexemeLength, source.Substring(startIndex + 1, lexemeLength - 1), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
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
                            tokens.Add(new Token(Token.Type.Error, startIndex, lexemeLength, source.Substring(startIndex, lexemeLength), line));
                            logger?.Log(LogLevel.Error, $"{tokens.Last()} ERROR: Invalid number, cannot end a number with a '.'");
                        }
                        else
                        {
                            tokens.Add(new Token(Token.Type.Number, startIndex, lexemeLength, source.Substring(startIndex, lexemeLength), line));
                            logger?.Log(LogLevel.Info, tokens.Last().ToString());
                        }

                        break;
                    }

                    case ' ':
                    case '\r':
                    case '\t':
                        // Ignore whitespace
                        break;
                    case '\n':
                        line++;
                        break;

                    default:
                        hasError = true;
                        errorReport?.Report($"Unrecognized character {source[currentIndex]}", currentIndex, 1);
                        tokens.Add(new Token(Token.Type.Error, currentIndex, 1, source.Substring(currentIndex, 1), line));
                        logger?.Log(LogLevel.Error, $"{tokens.Last()} Unrecognized character {source[currentIndex]}");
                        break;
                }

                currentIndex++;
            }

            tokens.Add(new Token(Token.Type.Eof, currentIndex, 0, string.Empty, line));
            logger?.Log(LogLevel.Info, tokens.Last().ToString());

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
}