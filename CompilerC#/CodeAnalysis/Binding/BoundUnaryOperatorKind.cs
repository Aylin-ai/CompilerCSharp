namespace CompilerCSharp.CodeAnalysis.Binding
{
    //Перечисление видов унарного оператора
    internal enum BoundUnaryOperatorKind{
        Identity,
        Negation,
        LogicalNegation
    }
}