using System;
using System.Collections.Generic;
using Tooling.Logging;
using Utils.Extensions;

namespace Tooling.StaticData.Bytecode
{
    public class InstructionSet
    {
        public readonly List<byte>   Instructions = new();
        public readonly List<object> Constants    = new();
    }

    public abstract class Expression
    {
    }

    public interface IPrefixParselet
    {
        Expression Parse(Compiler compiler, Token token);
    }

    public interface IInfixParselet
    {
        Expression Parse(Compiler compiler, Expression left, Token token);
    }

    public enum Precedence
    {
        None,
        Assignment, // =
        Or,         // or
        And,        // and
        Equality,   // == !=
        Comparison, // < > <= >=
        Term,       // + -
        Factor,     // * /
        Unary,      // ! -
        Call,       // . ()
        Primary
    }


    /// <summary>
    /// Compiles tokens parsed from the <see cref="Scanner"/> into <see cref="Bytecode"/> operations
    /// 
    /// Based on this grammar:
    /// 
    /// program -> declaration* EOF
    ///
    /// declaration ->  varDecl
    ///              |  funcDecl
    ///              |  statement 
    ///
    /// funDecl -> 'func' function 
    /// varDecl -> 'var' IDENTIFIER '=' expression 
    ///
    /// statement -> exprStmt
    ///            | forStmt
    ///            | ifStmt
    ///            | printStmt
    ///            | returnStmt
    ///            | whileStmt
    ///            | block 
    ///
    /// expStmt    -> expression ';'
    /// forStmt    -> 'for' '(' [ varDecl | exprStmt | ';' ] expression? ';' expression? ')' statement
    /// ifStmt     -> 'if'  '(' expression ')' statement [ 'else' statement ]?
    /// printStmt  -> 'print' expression ';'
    /// returnStmt -> 'return' expression? ';'
    /// whileStmt  -> 'while' '(' expression ')' statement
    /// block      -> '{' declaration* '}'
    ///
    /// expression -> assignment
    /// assignment -> [ call '.' ]? IDENTIFIER '=' assignment
    ///             | logic_or
    /// logic_or   -> logic_and [ 'or' logic_and ]*
    /// logic_and  -> equality [ 'and' equality ]*
    /// equality   -> comparison [ [ '!=' | '==' ] comparison ]*
    /// comparison -> term [ [ '>' | '>=' | '<' | '<=' ] term ]*
    /// term       -> factor [ [ '-' | '+' ] factor ]*
    /// factor     -> unary [ [ '/' | '*' ] unary ]*
    ///
    /// unary      -> [ '!' | '-' ] unary | call
    /// call       -> primary [ '(' arguments? ')' | '.' IDENTIFIER ]*
    /// primary    -> 'true' | 'false' | 'null' | 'this'
    ///             | NUMBER | STRING | IDENTIFIER | '(' expression ')'
    ///
    /// function   -> IDENTIFIER '(' parameters? ')' block
    /// parameters -> IDENTIFIER [ ',' IDENTIFIER ]*
    /// arguments  -> expression [ ',' expression ]*
    ///
    /// NUMBER     -> DIGIT+ [ '.' DIGIT+ ]?
    /// STRING     -> '"' [^"]* '"'
    /// IDENTIFIER -> ALPHA [ ALPHA | DIGIT ]*
    /// ALPHA      -> 'a'...'z' | 'A' ... 'Z'  | '_' 
    /// DIGIT      -> '0' ... '9'
    /// </summary>
    public class Compiler
    {
        private ILogger        logger;
        private InstructionSet instructionSet;

        /// <summary>
        /// The input list of tokens to parse from
        /// </summary>
        private List<Token> tokens;

        /// <summary>
        /// Index of where which token we are reading next from <see cref="tokens"/>
        /// </summary>
        private int tokenIndex;

        private Token previous;
        private Token current;
        private bool  hadError;
        private bool  panicMode;

        public bool Interpret(List<Token> tokens, out InstructionSet instructionSet, ILogger logger = null)
        {
            instructionSet = new InstructionSet();
            if (tokens.IsNullOrEmpty())
            {
                return true;
            }

            this.tokens         = tokens;
            this.instructionSet = instructionSet;
            this.logger         = logger;
            tokenIndex          = 0;

            Advance();

            while (!MatchThenAdvance(Token.Type.Eof))
            {
                Parse();
            }

            this.instructionSet.Instructions.Add((byte)Bytecode.Return);

            return !hadError;
        }

        private void Parse()
        {
            Declaration();

            if (panicMode)
            {
                Synchronize();
            }
        }

        private void Declaration()
        {
            if (MatchThenAdvance(Token.Type.Function))
            {
                FuncDeclaration();
            }
            else if (MatchThenAdvance(Token.Type.Var))
            {
                VarDeclaration();
            }
            else
            {
                Statement();
            }
        }

        private void FuncDeclaration()
        {
            Consume(Token.Type.Identifier, "Expected function name");
            Consume(Token.Type.LeftParen, "Expected left parenthesis");
            Parameters();
            Consume(Token.Type.RightParen, "Expected right parenthesis");
            Block();
        }

        private void Parameters()
        {
            Consume(Token.Type.Identifier, "Expected parameter name");
            if (MatchThenAdvance(Token.Type.Comma))
            {
                Parameters();
            }
        }

        private void Block()
        {
            Consume(Token.Type.LeftBrace, "Expected block name");
            Declaration();
            Consume(Token.Type.RightBrace, "Expected right brace");
        }

        private void VarDeclaration()
        {
            Consume(Token.Type.Identifier, "expected variable name");
            Consume(Token.Type.Equal, "Expected assignment operator");
            Expression();
        }

        private void Statement()
        {
            switch (current.TokenType)
            {
                case Token.Type.For:
                    ForStatement();
                    break;
                case Token.Type.If:
                    IfStatement();
                    break;
                case Token.Type.Print:
                    PrintStatement();
                    break;
                case Token.Type.Return:
                    ReturnStatement();
                    break;
                case Token.Type.While:
                    WhileStatement();
                    break;
                case Token.Type.LeftBrace:
                    Block();
                    break;
                default:
                    ExpressionStatement();
                    break;
            }
        }

        private void ForStatement()
        {
            Consume(Token.Type.For);
            Consume(Token.Type.LeftParen, "Expected left parenthesis");
            switch (current.TokenType)
            {
                case Token.Type.Var:
                    VarDeclaration();
                    break;
                case Token.Type.Semicolon:
                    Consume(Token.Type.Semicolon);
                    break;
                default:
                    ExpressionStatement();
                    break;
            }

            //TODO: How do check 0 or 1 expressions?
            Expression();
            Consume(Token.Type.Semicolon, "Expected semicolon");
            //TODO: How do check 0 or 1 expressions?
            Expression();
            Consume(Token.Type.RightParen, "Expected right parenthesis");
            Statement();
        }

        private void IfStatement()
        {
            Consume(Token.Type.If);
            Consume(Token.Type.LeftParen, "Expected left parenthesis");
            Expression();
            Consume(Token.Type.RightParen, "Expected right parenthesis");
            Statement();
            if (MatchThenAdvance(Token.Type.Else))
            {
                Statement();
            }
        }

        private void PrintStatement()
        {
            Consume(Token.Type.Print);
            Expression();
            Consume(Token.Type.Semicolon, "Expected semicolon");
        }

        private void ReturnStatement()
        {
            Consume(Token.Type.Return);
            //TODO: How do check 0 or 1 expressions?
            Expression();
            Consume(Token.Type.Semicolon, "Expected semicolon");
        }

        private void WhileStatement()
        {
            Consume(Token.Type.While);
            Consume(Token.Type.LeftParen, "Expected left parenthesis");
            Expression();
            Consume(Token.Type.RightParen, "Expected right parenthesis");
            Statement();
        }


        /// <summary>
        /// Resets the panic mode flag so we can continue parsing.
        /// Then advances the current token to the next declaration
        /// </summary>
        private void Synchronize()
        {
            panicMode = false;

            while (current.TokenType != Token.Type.Eof)
            {
                if (previous.TokenType == Token.Type.Semicolon)
                {
                    return;
                }

                switch (current.TokenType)
                {
                    case Token.Type.Function:
                    case Token.Type.Var:
                    case Token.Type.For:
                    case Token.Type.If:
                    case Token.Type.While:
                    case Token.Type.Return:
                    case Token.Type.Print:
                        return;
                }

                Advance();
            }
        }

        private void EmitByte(Bytecode code)
        {
            instructionSet.Instructions.Add((byte)code);
        }

        private bool MatchThenAdvance(Token.Type tokenType)
        {
            if (Match(tokenType))
            {
                Advance();
                return true;
            }

            return false;
        }

        private bool Match(Token.Type tokenType)
        {
            return current.TokenType == tokenType;
        }

        private void ExpressionStatement()
        {
            Expression();
            Consume(Token.Type.Semicolon, "Expected semicolon");
        }

        private void Expression(Precedence precedence = Precedence.None)
        {
        }

        private void Advance()
        {
            previous = current;
            while (true)
            {
                current = tokens[tokenIndex++];
                if (current.TokenType != Token.Type.Error)
                {
                    break;
                }

                ErrorAtCurrent($"Error at: {current.Start}:{current.Length}");
            }
        }

        /// <summary>
        /// Consumes the next token, if the token does not match <see cref="tokenType"/> then an error is fired with the <see cref="errorMessage"/>
        /// </summary>
        private void Consume(Token.Type tokenType, string errorMessage = "")
        {
            if (current.TokenType != tokenType)
            {
                ErrorAtCurrent(errorMessage);
                return;
            }

            Advance();
        }

        private void ErrorAtCurrent(string message)
        {
            if (panicMode)
            {
                return;
            }

            ErrorAt(previous, message);
        }

        private void ErrorAt(Token token, string message)
        {
            message += token.TokenType switch
            {
                Token.Type.Eof   => " at end.",
                Token.Type.Error => string.Empty,
                _                => $" at {token.Lexeme}"
            };

            hadError  = true;
            panicMode = true;
            logger?.Log(LogLevel.Error, message);
        }

        private void Grouping()
        {
        }

        private void Call()
        {
        }

        private void Unary()
        {
        }

        private void Binary()
        {
        }

        private void Dot()
        {
        }

        private void Variable()
        {
        }

        private void String()
        {
        }

        private void Number()
        {
        }

        private void And()
        {
        }

        private void Or()
        {
        }

        private void This()
        {
        }

        private void Super()
        {
        }

        private void Literal()
        {
        }

        private class ParseRule
        {
            public Action     PrefixRule;
            public Action     InfixRule;
            public Precedence Precedence;
        }

        private ParseRule GetParseRule(Token.Type tokenType)
        {
            return tokenType switch
            {
                Token.Type.LeftParen    => new ParseRule { PrefixRule = Grouping, InfixRule = Call, Precedence   = Precedence.Call },
                Token.Type.RightParen   => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.LeftBrace    => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.RightBrace   => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Comma        => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Dot          => new ParseRule { PrefixRule = null, InfixRule     = Dot, Precedence    = Precedence.None },
                Token.Type.Minus        => new ParseRule { PrefixRule = Unary, InfixRule    = Binary, Precedence = Precedence.Term },
                Token.Type.Plus         => new ParseRule { PrefixRule = null, InfixRule     = Binary, Precedence = Precedence.Term },
                Token.Type.Semicolon    => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Slash        => new ParseRule { PrefixRule = null, InfixRule     = Binary, Precedence = Precedence.Factor },
                Token.Type.Star         => new ParseRule { PrefixRule = null, InfixRule     = Binary, Precedence = Precedence.Factor },
                Token.Type.Bang         => new ParseRule { PrefixRule = Unary, InfixRule    = null, Precedence   = Precedence.None },
                Token.Type.BangEqual    => new ParseRule { PrefixRule = null, InfixRule     = Binary, Precedence = Precedence.Equality },
                Token.Type.Equal        => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.EqualEqual   => new ParseRule { PrefixRule = null, InfixRule     = Binary, Precedence = Precedence.Equality },
                Token.Type.Greater      => new ParseRule { PrefixRule = null, InfixRule     = Binary, Precedence = Precedence.Comparison },
                Token.Type.GreaterEqual => new ParseRule { PrefixRule = null, InfixRule     = Binary, Precedence = Precedence.Comparison },
                Token.Type.Less         => new ParseRule { PrefixRule = null, InfixRule     = Binary, Precedence = Precedence.Comparison },
                Token.Type.LessEqual    => new ParseRule { PrefixRule = null, InfixRule     = Binary, Precedence = Precedence.Comparison },
                Token.Type.Identifier   => new ParseRule { PrefixRule = Variable, InfixRule = null, Precedence   = Precedence.None },
                Token.Type.String       => new ParseRule { PrefixRule = String, InfixRule   = null, Precedence   = Precedence.None },
                Token.Type.Number       => new ParseRule { PrefixRule = Number, InfixRule   = null, Precedence   = Precedence.None },
                Token.Type.And          => new ParseRule { PrefixRule = null, InfixRule     = And, Precedence    = Precedence.And },
                Token.Type.Or           => new ParseRule { PrefixRule = null, InfixRule     = Or, Precedence     = Precedence.Or },
                Token.Type.If           => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Else         => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.True         => new ParseRule { PrefixRule = Literal, InfixRule  = null, Precedence   = Precedence.None },
                Token.Type.False        => new ParseRule { PrefixRule = Literal, InfixRule  = null, Precedence   = Precedence.None },
                Token.Type.Null         => new ParseRule { PrefixRule = Literal, InfixRule  = null, Precedence   = Precedence.None },
                Token.Type.While        => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.For          => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Function     => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Print        => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Return       => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Super        => new ParseRule { PrefixRule = Super, InfixRule    = null, Precedence   = Precedence.None },
                Token.Type.This         => new ParseRule { PrefixRule = This, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Var          => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Class        => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Eof          => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                Token.Type.Error        => new ParseRule { PrefixRule = null, InfixRule     = null, Precedence   = Precedence.None },
                _                       => null
            };
        }
    }
}