namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий выражение с использованием переменной.
    Содержит в себе токен переменной.
    */
    public sealed class NameExpressionSyntax : ExpressionSyntax{
        public NameExpressionSyntax(SyntaxToken identifierToken){
            IdentifierToken = identifierToken;
        }

        public SyntaxToken IdentifierToken { get; }

        public override SyntaxKind Kind => SyntaxKind.NameExpression;
    }
}