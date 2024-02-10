namespace CompilerCSharp.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий узел числа, от которого идет
    узел, представляющий числовой токен.
    В качестве дочерних узлов содержит один узел,
    представляющий число
    */
    sealed class LiteralExpressionSyntax : ExpressionSyntax{
        public LiteralExpressionSyntax(SyntaxToken literalToken){
            LiteralToken = literalToken;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken LiteralToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken;
        }
    }
}