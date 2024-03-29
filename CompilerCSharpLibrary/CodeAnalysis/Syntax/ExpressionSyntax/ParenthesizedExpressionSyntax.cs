using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{
    /*
    Класс, представляющий выражение в скобках
    */
    public sealed class ParenthesizedExpressionSyntax : BaseExpressionSyntax{
        public ParenthesizedExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken openParenthesisToken, 
        BaseExpressionSyntax expression, SyntaxToken closeParenthesisToken)
            : base(syntaxTree)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }
        public override SyntaxKind Kind => SyntaxKind.ParethesizedExpression;
        public SyntaxToken OpenParenthesisToken { get; }
        public BaseExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }
    }
}