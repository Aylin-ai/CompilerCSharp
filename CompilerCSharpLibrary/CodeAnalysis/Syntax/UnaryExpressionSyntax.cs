namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий узел, от которого исходит 2 узла,
    являющиеся оператором и операндом унарного выражения.
    Операнд может ветвиться дальше, то есть
    сам являться деревом, т.к. класса ExpressionSyntax.
    */
    public sealed class UnaryExpressionSyntax : ExpressionSyntax{
        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand){
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Operand { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return Operand;
        }
    }
}