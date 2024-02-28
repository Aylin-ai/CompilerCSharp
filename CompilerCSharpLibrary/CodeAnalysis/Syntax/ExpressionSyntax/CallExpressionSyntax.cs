using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{
    public sealed class CallExpressionSyntax : BaseExpressionSyntax
    {
        public CallExpressionSyntax(SyntaxTree syntaxTree, SyntaxToken identifier, 
        SyntaxToken openParenthesisToken, SeparatedSyntaxList<BaseExpressionSyntax> arguments, 
        SyntaxToken closeParenthesisToken)
            : base(syntaxTree)
        {
            Identifier = identifier;
            OpenParenthesisToken = openParenthesisToken;
            Arguments = arguments;
            CloseParenthesisToken = closeParenthesisToken;
        }
        public override SyntaxKind Kind => SyntaxKind.CallExpression;

        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public SeparatedSyntaxList<BaseExpressionSyntax> Arguments { get; }
        public SyntaxToken CloseParenthesisToken { get; }
    }
}