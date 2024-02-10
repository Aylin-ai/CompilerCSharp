namespace CompilerCSharp.CodeAnalysis.Binding
{
    /*
    Класс, являющийся представлением бинарного выражения.
    Содержит в себе бинарный оператор и 2 операнда слева и справа от него.
    Кроме этого содержит тип выражения(int, bool и т.п.) 
    и вид (в данном случае UnaryExpression - унарное выражение)
    */
    internal sealed class BoundBinaryExpression : BoundExpression{
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right){
            Left = left;
            Op = op;
            Right = right;
        }

        public override Type Type => Op.Type;

        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;

        public BoundExpression Left { get; }
        public BoundBinaryOperator Op { get; }
        public BoundExpression Right { get; }
    }
}