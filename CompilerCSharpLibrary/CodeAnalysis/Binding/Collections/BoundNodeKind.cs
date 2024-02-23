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

        //Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,
    }
}