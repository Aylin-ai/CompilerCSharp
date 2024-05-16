namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Collections
{
    public enum BoundNodeKind
    {

        #region Statements

        BlockStatement,
        NopStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        DoWhileStatement,
        ForStatement,
        GotoStatement,
        LabelStatement,
        ConditionalGotoStatement,
        ReturnStatement,

        #endregion

        #region Expression

        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
        ErrorExpression,
        CallExpression,
        ConversionExpression,
        CompoundAssignmentExpression,

        #endregion

    }
}