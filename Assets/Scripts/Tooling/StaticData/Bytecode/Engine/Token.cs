namespace Tooling.StaticData.Bytecode
{
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

        public readonly int Line;

        public Token(Type tokenType, int start, int length, string lexeme, int line)
        {
            TokenType = tokenType;
            Start = start;
            Length = length;
            Lexeme = lexeme;
            Line = line;
        }

        public override string ToString()
        {
            return $"{Line} — {TokenType} \tstart:{Start}\t len:{Length} \tlexeme:{Lexeme}";
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
}