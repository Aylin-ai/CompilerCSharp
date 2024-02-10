namespace CompilerCSharp.CodeAnalysis.Binding
{
    /*
    Объяснения, зачем нужно абстрактное дерево: 
    */
    internal enum BoundNodeKind{
        UnaryExpression,
        LiteralExpression,
        BinaryExpression
    }
}