namespace CompilerCSharp.CodeAnalysis
{
    /*
    Класс, представляющий узел числа, от которого идет
    узел, представляющий числовой токен.
    В качестве дочерних узлов содержит один узел,
    представляющий число
    */
    sealed class NumberExpressionSyntax : ExpressionSyntax{
        public NumberExpressionSyntax(SyntaxToken numberToken){
            NumberToken = numberToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;

        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }
}