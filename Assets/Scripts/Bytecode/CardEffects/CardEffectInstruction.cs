namespace Bytecode
{
    public enum CardEffectInstruction
    {
        LITERAL_LONG,
        LITERAL_FLOAT,
        LITERAL_BOOL,
        LITERAL_STRING,

        BINARY_ARITHMETIC_OPERATOR,

        /*         GET_CLASS,
                GET_METHOD,
                GET_PROPERTY,

                WHERE,
                BINARY_LOGICAL_OPERATOR,
                UNARY_LOGICAL_OPERATOR,

                GET_CHARACTERS_IN_FIGHT, */
    }

    public enum ArithmeticOperator
    {
        ADD,
        SUB,
        MUL,
        DIV,
        POW
    }
}