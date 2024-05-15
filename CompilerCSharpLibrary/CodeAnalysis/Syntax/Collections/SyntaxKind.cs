namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections
{
    //Виды токенов
    public enum SyntaxKind
    {
        BadToken,

        #region Trivia

        WhiteSpaceTrivia,
        SingleLineCommentTrivia,
        LineBreakTrivia,
        MultiLineCommentTrivia,
        SkippedTextTrivia,
        SharpToken,

        #endregion

        #region Tokens

        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
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
        SemiColonToken,
        PipeEqualsToken,
        AmpersandEqualsToken,
        HatEqualsToken,
        SlashEqualsToken,
        StarEqualsToken,
        MinusEqualsToken,
        PlusEqualsToken,

        #endregion

        #region Expressions

        LiteralExpression,
        BinaryExpression,
        ParethesizedExpression,
        UnaryExpression,
        NameExpression,
        AssignmentExpression,
        CallExpression,

        #endregion

        #region Nodes

        CompilationUnit,
        FunctionDeclaration,
        Parameter,
        ElseClause,
        TypeClause,

        #endregion

        #region Statements

        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        DoWhileStatement,
        ForStatement,
        GlobalStatement,
        BreakStatement,
        ContinueStatement,
        ReturnStatement,

        #endregion

        #region Keywords

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
        FunctionKeyword,
        BreakKeyword,
        ContinueKeyword,
        ReturnKeyword,

        #endregion

    }
}