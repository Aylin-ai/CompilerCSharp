namespace CompilerCSharp.CodeAnalysis.Binding
{
    /*
    Класс, являющийся отражением выражения с литерой.
    Содержит в себе вид выражения(LiteralExpression), 
    тип литеры внутри(Value.GetType()) и ее значение(Value)
    */
    internal sealed class BoundLiteralExpression : BoundExpression{
        public BoundLiteralExpression(object value){
            Value = value;
        }

        public object Value { get; }

        public override Type Type => Value.GetType();

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;
    }
}