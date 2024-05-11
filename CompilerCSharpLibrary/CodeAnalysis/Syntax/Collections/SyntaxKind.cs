namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections
{
    //Виды токенов
    public enum SyntaxKind{
        BadToken,

        //Trivia
        WhiteSpaceTrivia,
        SingleLineCommentTrivia,
        LineBreakTrivia,
        MultiLineCommentTriva,
        SkippedTextTrivia,

        //Tokens
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
        FunctionDeclaration,
        Parameter,
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
        GlobalStatement,
        BreakStatement,
        ContinueStatement,
        ReturnStatement,
        
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
        FunctionKeyword,
        BreakKeyword,
        ContinueKeyword,
        ReturnKeyword,
    }
}