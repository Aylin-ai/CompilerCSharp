namespace CompilerCSharp.CodeAnalysis
{
    //Виды токенов
    enum SyntaxKind{
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
        NumberExpression,
        BinaryExpression,
        ParethesizedExpression
    }
}