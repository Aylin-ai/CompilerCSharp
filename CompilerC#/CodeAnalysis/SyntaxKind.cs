namespace CompilerCSharp.CodeAnalysis
{
    //Виды токенов
    enum SyntaxKind{
        //Tokens
        NumberToken,
        WhiteSpaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,

        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParethesizedExpression,
        UnaryExpression
    }
}