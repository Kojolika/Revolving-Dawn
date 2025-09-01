using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tooling.Logging;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI.Bytecode
{
    public class InstructionSet
    {
        public readonly List<byte>    Instructions = new();
        public readonly List<CObject> Constants    = new();
    }

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

    public struct Local
    {
        /// <summary>
        /// Holds the name of the local var in the lexeme property
        /// </summary>
        public Token Token;

        /// <summary>
        /// The depth of which this local val is created in.
        /// A depth of -1 means it's not initialized yet.
        /// </summary>
        public int Depth;
    }

    /// <summary>
    /// Compiler object
    /// </summary>
    public struct CObject
    {
        public ValueType Type;
        public Value     Value;
    }

    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct Value
    {
        /// <summary>
        /// Stores number values
        /// </summary>
        [FieldOffset(0)]
        public double Number;

        /// <summary>
        /// Stores boolean values
        /// </summary>
        [FieldOffset(0)]
        public bool Boolean;

        /// <summary>
        /// Stores a pointer to all other reference types
        /// </summary>
        [FieldOffset(0)]
        public object Object;
    }

    public enum ValueType
    {
        Null,
        Boolean,
        Number,
        String,
        Table,
        Function
    }

    public enum FunctionType
    {
        Function,
        Initializer,
        Method,
        Script
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

        /// <summary>
        /// The previous token being evaluated.
        /// </summary>
        private Token previous;

        /// <summary>
        /// The current token being evaluated.
        /// </summary>
        private Token current;

        /// <summary>
        /// Stores the current local variables
        /// </summary>
        private List<Local> Locals = new();

        /// <summary>
        /// The current scope depth for variables
        /// </summary>
        private int scopeDepth;

        private bool hadError;
        private bool panicMode;

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
            byte global = ParseVariable();
            MarkInitialized();
            Function(FunctionType.Function);
            DefineVariable(global);
        }

        private void Function(FunctionType functionType)
        {
        }

        /// <summary>
        /// Only used to defined functions so we can call them before they're fully initialized
        /// </summary>
        private void DefineVariable(byte global)
        {
            if (scopeDepth > 0)
            {
                MarkInitialized();
                return;
            }
            
            EmitByte(Bytecode.DefineGlobal);
            EmitByte(global);
        }

        private void BeginScope()
        {
            scopeDepth++;
            while (Locals.Count > 0 && Locals[^1].Depth > scopeDepth)
            {
                EmitByte(Bytecode.Pop);
                Locals.RemoveAt(Locals.Count - 1);
            }
        }

        private void EndScope()
        {
            scopeDepth--;
        }

        // TODO: what is this doing?
        // Marks the last created variable as initialized by setting its depth?
        private void MarkInitialized()
        {
            if (scopeDepth == 0) return;
            if (Locals.Count <= 0) return;

            var lastLocal = Locals[^1];
            Locals[^1] = new Local
            {
                Token = lastLocal.Token,
                Depth = scopeDepth,
            };
        }

        private byte ParseVariable()
        {
            Consume(Token.Type.Identifier, "Expected function name");

            DeclareVariable();

            if (scopeDepth > 0)
            {
                return 0;
            }

            return AddConstant(new CObject
            {
                Type = ValueType.Function,
                // Store the var identifier as the reference value
                Value = new Value { Object = previous.Lexeme }
            });
        }

        private void DeclareVariable()
        {
            // At runtime, locals aren’t looked up by name.
            // There’s no need to stuff the variable’s name into the constant table, so if the declaration is inside a local scope,
            // we return a dummy table index instead
            if (scopeDepth == 0)
            {
                return;
            }

            string name = previous.Lexeme;
            foreach (var local in Locals)
            {
                // Only check local variables that are declared in this scope
                if (local.Depth != -1 && local.Depth < scopeDepth)
                {
                    continue;
                }

                if (name == local.Token.Lexeme)
                {
                    ErrorAt(previous, "Already a variable with this name in this scope.");
                }
            }

            AddLocal(previous);
        }

        private void AddLocal(Token token)
        {
            if (Locals.Count >= byte.MaxValue)
            {
                ErrorAt(previous, "Too many local variables in this scope.");
                return;
            }

            var local = new Local
            {
                Token = token,
                Depth = scopeDepth = -1
            };

            Locals.Add(local);
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
                    BeginScope();
                    Block();
                    EndScope();
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
            BeginScope();
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

            int loopStartIndex = instructionSet.Instructions.Count - 1;
            int exitJump       = -1;

            if (!MatchThenAdvance(Token.Type.Semicolon))
            {
                Expression();
                Consume(Token.Type.Semicolon, "Expected semicolon");

                exitJump = EmitJump(Bytecode.JumpIfFalse);
                // Pop the condition off the stack after evaluating
                EmitByte(Bytecode.Pop);
            }

            if (!MatchThenAdvance(Token.Type.RightParen))
            {
                int bodyJump       = EmitJump(Bytecode.Jump);
                int incrementStart = instructionSet.Instructions.Count - 1;

                Expression();
                // Pop the condition off the stack after evaluating
                EmitByte(Bytecode.Pop);
                Consume(Token.Type.RightParen, "Expected right parenthesis");

                EmitLoop(loopStartIndex);
                loopStartIndex = incrementStart;
                PatchJump(bodyJump);
            }

            Statement();
            EmitLoop(loopStartIndex);
            if (exitJump != -1)
            {
                PatchJump(exitJump);
                EmitByte(Bytecode.Pop);
            }

            EndScope();
        }

        private void EmitLoop(int loopStartIndex)
        {
            EmitByte(Bytecode.Loop);

            int offset = instructionSet.Instructions.Count - loopStartIndex + 2;
            if (offset > ushort.MaxValue)
            {
                ErrorAt(previous, "Loop value too large in this scope.");
            }

            // Store the offset on the vm's input as a 2 byte unsigned int16
            var bytes = BitConverter.GetBytes(offset);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            EmitByte(bytes[0]);
            EmitByte(bytes[1]);
        }

        private int EmitJump(Bytecode jumpCode)
        {
            if (jumpCode is not (Bytecode.Jump or Bytecode.JumpIfFalse))
            {
                return -1;
            }

            EmitByte(jumpCode);

            // We'll patch these instruction offsets later, we're storing the offset
            // as a short which is why we emit 2 bytes
            EmitByte(byte.MaxValue);
            EmitByte(byte.MaxValue);

            return instructionSet.Instructions.Count - 3;
        }

        private void PatchJump(int offset)
        {
            int jump = instructionSet.Instructions.Count - offset - 2;
            if (jump > ushort.MaxValue)
            {
                ErrorAtCurrent("Too much code to jump over");
            }

            // Update the bytes that store the short value with the jump value
            instructionSet.Instructions[offset] = (byte)((jump >> 8) & byte.MaxValue);
            instructionSet.Instructions[offset] = (byte)(jump & byte.MaxValue);
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
            Expression();
            Consume(Token.Type.Semicolon, "Expected semicolon");
        }

        private void WhileStatement()
        {
            logger?.Log(LogLevel.Info, "While —");
            Consume(Token.Type.While);
            int loopStart = instructionSet.Instructions.Count - 1;
            Consume(Token.Type.LeftParen, "Expected left parenthesis");
            Expression();
            Consume(Token.Type.RightParen, "Expected right parenthesis");

            int exitJump = EmitJump(Bytecode.JumpIfFalse);
            EmitByte(Bytecode.Pop);
            Statement();

            EmitLoop(loopStart);

            PatchJump(exitJump);
            EmitByte(Bytecode.Pop);
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
            EmitByte(Bytecode.Pop);
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

            bool canAssign = precedence <= Precedence.Assignment;
            prefixRule.Invoke(canAssign);

            while (precedence <= GetParseRule(current.TokenType).Precedence)
            {
                logger?.Log(LogLevel.Info, "While iteration\n" +
                                           $"\tprecedence={precedence} to precedence={GetParseRule(current.TokenType).Precedence}");
                Advance();
                var infixRule = GetParseRule(previous.TokenType).InfixRule;
                if (infixRule != null)
                {
                    infixRule.Invoke(canAssign);
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

        private void Grouping(bool canAssign)
        {
            Expression();
            Consume(Token.Type.RightParen, "Expected right parenthesis after expression");
        }

        private void Call(bool canAssign)
        {
        }

        private void Unary(bool canAssign)
        {
            var tokenType = previous.TokenType;
            Expression();
            if (tokenType is Token.Type.Minus or Token.Type.Bang)
            {
                EmitByte(Bytecode.Negate);
            }
        }

        private void Binary(bool canAssign)
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

        private void Dot(bool canAssign)
        {
        }

        private void Variable(bool canAssign)
        {
            NamedVariable(previous, canAssign);
        }

        private void NamedVariable(Token token, bool canAssign)
        {
            int arg = ResolveLocal(token);

            if (canAssign && MatchThenAdvance(Token.Type.Equal))
            {
                Expression();
                EmitByte(Bytecode.SetLocal);
            }
            else
            {
                EmitByte(Bytecode.GetLocal);
            }

            EmitByte((byte)arg);
        }

        /// <param name="token"> The token that contains the given identifier </param>
        /// <returns> the index of a local variable of the given name, if not found returns -1 </returns>
        private int ResolveLocal(Token token)
        {
            for (var index = Locals.Count - 1; index >= 0; index--)
            {
                var local = Locals[index];
                if (local.Depth == -1)
                {
                    ErrorAtCurrent("Can't read local variable in its own initializer.");
                }

                if (token.Lexeme == local.Token.Lexeme)
                {
                    return index;
                }
            }

            ErrorAtCurrent("Can't read local variable in its own initializer.");

            return -1;
        }

        private void String(bool canAssign)
        {
        }

        /// <summary>
        /// Using boxing to store values in an array.
        /// TODO: Do we need to free variables ever? Is the boxing a perf problem?
        /// Can use StructLayout in the future to reduce boxing
        /// </summary>
        private void Number(bool canAssign)
        {
            float.TryParse(previous.Lexeme, out float result);
            EmitByte(Bytecode.Constant);
            ushort lookup = AddConstant(new CObject
            {
                Type  = ValueType.Number,
                Value = new Value { Number = result }
            });
            EmitByte((byte)lookup);
        }

        /// <summary>
        /// Adds a value to look up table and returns the index to look up the value.
        /// </summary>
        private byte AddConstant(CObject constant)
        {
            instructionSet.Constants.Add(constant);

            if (instructionSet.Constants.Count > ushort.MaxValue)
            {
                ErrorAt(previous, "Too many constants!");
            }

            return (byte)(instructionSet.Constants.Count - 1);
        }

        private void And(bool canAssign)
        {
            int endJump = EmitJump(Bytecode.JumpIfFalse);

            EmitByte(Bytecode.Pop);
            Expression(Precedence.And);

            PatchJump(endJump);
        }

        private void Or(bool canAssign)
        {
            int elseJump = EmitJump(Bytecode.JumpIfFalse);
            int endJump  = EmitJump(Bytecode.Jump);

            PatchJump(elseJump);
            EmitByte(Bytecode.Pop);

            Expression(Precedence.Or);
            PatchJump(endJump);
        }

        private void This(bool canAssign)
        {
        }

        private void Super(bool canAssign)
        {
        }

        private void Literal(bool canAssign)
        {
        }

        private class ParseRule
        {
            /// <summary>
            /// The rule that corresponds to a prefix of an expression.
            /// The bool parameter is whether this can be an expression that assigns a value.
            /// </summary>
            public Action<bool> PrefixRule;

            /// <summary>
            /// The rule that corresponds to an infix of an expression.
            /// The bool parameter is whether this can be an expression that assigns a value.
            /// </summary>
            public Action<bool> InfixRule;

            /// <summary>
            /// The precedence that's associated with this rule
            /// </summary>
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