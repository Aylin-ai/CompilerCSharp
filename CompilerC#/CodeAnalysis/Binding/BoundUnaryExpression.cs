namespace CompilerCSharp.CodeAnalysis.Binding
{
    /*
    Класс, являющийся представлением унарного выражения.
    Содержит в себе унарный оператор и само выражение.
    Кроме этого содержит тип выражения(int, bool и т.п.) 
    и вид (в данном случае UnaryExpression - унарное выражение)
    */
    internal sealed class BoundUnaryExpression : BoundExpression{
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand){
            Op = op;
            Operand = operand;
        }

        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }

        public override Type Type => Op.Type;

        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    }
}