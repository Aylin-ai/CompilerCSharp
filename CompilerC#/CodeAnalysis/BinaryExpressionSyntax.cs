namespace CompilerCSharp.CodeAnalysis
{
    /*
    Класс, представляющий узел, от которого исходит 3 узла,
    являющиеся левым операндом, оператором и правым операндом
    бинарного выражения.
    Левые и правые операнды могут ветвиться дальше, то есть
    сами являться деревьями, т.к. класса ExpressionSyntax.
    */
    sealed class BinaryExpressionSyntax : ExpressionSyntax{
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right){
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}