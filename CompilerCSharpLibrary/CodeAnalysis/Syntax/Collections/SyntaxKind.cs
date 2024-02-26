namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections
{
    //Виды токенов
    public enum SyntaxKind{
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
        NotEqualsToken,
        EqualsEqualsToken,
        GreaterOrEqualsToken,
        GreaterToken,
        LessOrEqualsToken,
        LessToken,
        EqualsToken,
        OpenBraceToken,
        CloseBraceToken,
        AmpersandToken,
        PipeToken,
        TildeToken,
        HatToken,
        StringToken,
        CommaToken,
        ColonToken,
       
        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParethesizedExpression,
        UnaryExpression,
        NameExpression,
        AssignmentExpression,
        CallExpression,

        //Nodes
        CompilationUnit,
        ElseClause,
        TypeClause,

        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        DoWhileStatement,
        ForStatement,

        //Keywords
        TrueKeyword,
        FalseKeyword,
        VarKeyword,
        LetKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,
        ForKeyword,
        ToKeyword,
        DoKeyword,
    }
}