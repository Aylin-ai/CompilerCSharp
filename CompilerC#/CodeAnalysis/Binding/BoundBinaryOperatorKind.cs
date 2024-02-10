namespace CompilerCSharp.CodeAnalysis.Binding
{
    //Список типов бинарного оператора
    internal enum BoundBinaryOperatorKind{
        Addition,
        Substraction,
        Multiplication,
        Division,
        LogicalAnd,
        LogicalOr,
        NotEquals,
        Equals,
    }
}