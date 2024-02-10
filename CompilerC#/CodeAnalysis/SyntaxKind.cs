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
        NumberExpression,
        BinaryExpression,
        ParethesizedExpression
    }
}