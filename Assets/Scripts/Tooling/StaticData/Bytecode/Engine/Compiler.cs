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

    // 1  +  1
    // 1  +
    // 0, 6
    //     

    public enum Precedence
    {
        None       = 0,
        Assignment = 1, // =
        Or         = 2, // or
        And        = 3, // and
        Equality   = 4, // == !=
        Comparison = 5, // < > <= >=
        Term       = 6, // + -
        Factor     = 7, // * /
        Unary      = 8, // ! -
        Call       = 9, // . ()
        Primary    = 10
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
            logger?.Log(LogLevel.Info, "==== Compiling ====");
            logger?.Log(LogLevel.Info, "Interpret —");
            instructionSet = new InstructionSet();
            if (tokens is { Count: 0 })
            {
                return true;
            }

            this.tokens         = tokens;
            this.instructionSet = instructionSet;
            this.logger         = logger;
            Advance();

            while (!MatchThenAdvance(Token.Type.Eof))
            {
                Parse();
            }

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
            logger?.Log(LogLevel.Info, "Declaration —");
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
            logger?.Log(LogLevel.Info, "Function Declaration —");
            Consume(Token.Type.Identifier, "Expected function name");
            Consume(Token.Type.LeftParen, "Expected left parenthesis");
            Parameters();
            Consume(Token.Type.RightParen, "Expected right parenthesis");
            Block();
        }

        private void Parameters()
        {
            logger?.Log(LogLevel.Info, "Parameters —");
            Consume(Token.Type.Identifier, "Expected parameter name");
            if (MatchThenAdvance(Token.Type.Comma))
            {
                Parameters();
            }
        }

        private void Block()
        {
            logger?.Log(LogLevel.Info, "Block —");
            Consume(Token.Type.LeftBrace, "Expected block name");
            Declaration();
            Consume(Token.Type.RightBrace, "Expected right brace");
        }

        private void VarDeclaration()
        {
            logger?.Log(LogLevel.Info, "Variable Declaration —");
            Consume(Token.Type.Identifier, "expected variable name");
            Consume(Token.Type.Equal, "Expected assignment operator");
            Expression();
        }

        private void Statement()
        {
            logger?.Log(LogLevel.Info, "Statement —");
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
            logger?.Log(LogLevel.Info, "For —");
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
            logger?.Log(LogLevel.Info, "If —");
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
            logger?.Log(LogLevel.Info, "Print —");
            Consume(Token.Type.Print);
            Expression();
            Consume(Token.Type.Semicolon, "Expected semicolon");
        }

        private void ReturnStatement()
        {
            logger?.Log(LogLevel.Info, "Return —");
            Consume(Token.Type.Return);
            //TODO: How do check 0 or 1 expressions?
            Expression();
            Consume(Token.Type.Semicolon, "Expected semicolon");
        }

        private void WhileStatement()
        {
            logger?.Log(LogLevel.Info, "While —");
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

        private void EmitByte(byte code)
        {
            instructionSet.Instructions.Add(code);
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
            logger?.Log(LogLevel.Info, $"Expression — {precedence}");
            Advance();
            var prefixRule = GetParseRule(previous.TokenType).PrefixRule;
            if (prefixRule == null)
            {
                ErrorAtCurrent($"Error, no prefix found for token. tokenType={previous.TokenType}, lexeme={previous.Lexeme}");
                return;
            }

            prefixRule.Invoke();

            while (precedence <= GetParseRule(current.TokenType).Precedence)
            {
                logger?.Log(LogLevel.Info, "While iteration\n" +
                                           $"\tprecedence={precedence} to precedence={GetParseRule(current.TokenType).Precedence}");
                Advance();
                var infixRule = GetParseRule(previous.TokenType).InfixRule;
                if (infixRule != null)
                {
                    infixRule.Invoke();
                }
                else
                {
                    break;
                }
            }
        }

        private void Advance()
        {
            previous = current;
            while (tokenIndex < tokens.Count)
            {
                current = tokens[tokenIndex];
                tokenIndex++;
                logger?.Log(LogLevel.Info, $"Advancing — previous={previous?.TokenType}, current={current.TokenType}");
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

            logger?.Log(LogLevel.Info, $"Consume — {errorMessage}");
            Advance();
        }

        private void ErrorAtCurrent(string message)
        {
            if (panicMode)
            {
                return;
            }

            ErrorAt(current, message);
        }

        private void ErrorAt(Token token, string message)
        {
            message += token.TokenType switch
            {
                Token.Type.Eof   => " at end.",
                Token.Type.Error => string.Empty,
                _                => $" at {token.Start}:{token.Start + token.Length}"
            };

            hadError  = true;
            panicMode = true;
            logger?.Log(LogLevel.Error, message);
        }

        private void Grouping()
        {
            Expression();
            Consume(Token.Type.RightParen, "Expected right parenthesis after expression");
        }

        private void Call()
        {
        }

        private void Unary()
        {
            var tokenType = previous.TokenType;
            Expression();
            if (tokenType is Token.Type.Minus or Token.Type.Bang)
            {
                EmitByte(Bytecode.Negate);
            }
        }

        private void Binary()
        {
            var tokenType = previous.TokenType;
            var parseRule = GetParseRule(tokenType);
            logger?.Log(LogLevel.Info,
                        $"Binary — previous tokenType={tokenType}, precedence={parseRule.Precedence}, next precedence={parseRule.Precedence + 1}");
            Expression(parseRule.Precedence + 1);
            switch (tokenType)
            {
                case Token.Type.Plus:
                    EmitByte(Bytecode.Add);
                    break;
                case Token.Type.Minus:
                    EmitByte(Bytecode.Subtract);
                    break;
                case Token.Type.Star:
                    EmitByte(Bytecode.Multiply);
                    break;
                case Token.Type.Slash:
                    EmitByte(Bytecode.Divide);
                    break;
            }
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

        /// <summary>
        /// Using boxing to store values in an array.
        /// TODO: Do we need to free variables ever? Is the boxing a perf problem?
        /// Can use StructLayout in the future to reduce boxing
        /// </summary>
        private void Number()
        {
            float.TryParse(previous.Lexeme, out float result);
            EmitByte(Bytecode.Constant);
            instructionSet.Constants.Add(result);
            EmitByte((byte)(instructionSet.Constants.Count - 1));
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