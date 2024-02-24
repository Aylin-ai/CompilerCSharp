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
       
        //Expressions
        LiteralExpression,
        BinaryExpression,
        ParethesizedExpression,
        UnaryExpression,
        NameExpression,
        AssignmentExpression,

        //Nodes
        CompilationUnit,
        ElseClause,

        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
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
    }
}