namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Collections
{
    //Список типов бинарного оператора
    public enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        LogicalAnd,
        LogicalOr,
        NotEquals,
        Equals,
        GreaterOrEquals,
        Greater,
        LessOrEquals,
        Less,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,
    }
}