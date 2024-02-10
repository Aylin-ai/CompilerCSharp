namespace CompilerCSharp.CodeAnalysis
{
    /*
    Класс, представляющий выражение в скобках
    */
    sealed class ParenthesizedExpressionSyntax : ExpressionSyntax{
        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression,
        SyntaxToken closeParenthesisToken){
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }
        public override SyntaxKind Kind => SyntaxKind.ParethesizedExpression;
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return Expression;
            yield return CloseParenthesisToken;
        }
    }
}