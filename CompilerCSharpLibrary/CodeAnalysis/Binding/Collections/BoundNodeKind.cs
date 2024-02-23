namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Collections
{
    /*
    Виды выражений (унарное, бинарное или обычная литера)
    */
    public enum BoundNodeKind{
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression
    }
}