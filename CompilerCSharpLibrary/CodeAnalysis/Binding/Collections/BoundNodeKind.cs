namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Collections
{
    /*
    Виды выражений (унарное, бинарное или обычная литера)
    */
    public enum BoundNodeKind{
        //Statements
        BlockStatement,
        ExpressionStatement,
        VariableDeclaration,
        IfStatement,
        WhileStatement,
        ForStatement,
        GotoStatement,
        LabelStatement,
        ConditionalGotoStatement,

        //Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
        ErrorExpression,
    }
}