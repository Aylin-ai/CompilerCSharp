namespace CompilerCSharp.CodeAnalysis.Binding
{
    /*
    Виды выражений (унарное, бинарное или обычная литера)
    */
    internal enum BoundNodeKind{
        UnaryExpression,
        LiteralExpression,
        BinaryExpression
    }
}