using System;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions
{
    /*
    Класс, являющийся отражением выражения с литерой.
    Содержит в себе вид выражения(LiteralExpression), 
    тип литеры внутри(Value.GetType()) и ее значение(Value)
    */
    public sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            if (value is bool)
                Type = TypeSymbol.Bool;
            else if (value is int)
                Type = TypeSymbol.Int;
            else if (value is string)
                Type = TypeSymbol.String;
            else
                throw new Exception($"Unexpected literal '{value}' of type '{value.GetType()}'");

            ConstantValue = new BoundConstant(value);
        }

        public object Value => ConstantValue.Value;
        public override BoundConstant ConstantValue { get; }
        public override TypeSymbol Type { get; }
        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    }
}