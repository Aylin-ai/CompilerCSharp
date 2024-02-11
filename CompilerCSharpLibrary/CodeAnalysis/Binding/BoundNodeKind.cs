namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    /*
    Виды выражений (унарное, бинарное или обычная литера)
    */
    public enum BoundNodeKind{
        UnaryExpression,
        LiteralExpression,
        BinaryExpression
    }
}