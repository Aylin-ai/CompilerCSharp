namespace CompilerCSharp.CodeAnalysis.Syntax
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
        IdentifierToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,

        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParethesizedExpression,
        UnaryExpression,

        //Keywords
        TrueKeyword,
        FalseKeyword,
    }
}