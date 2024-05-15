using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using System.Collections.Generic;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{
    public sealed class CallExpressionSyntax : BaseExpressionSyntax
    {
        public CallExpressionSyntax(SyntaxTree syntaxTree,
                                    SyntaxToken identifier,
                                    SyntaxToken openParenthesisToken,
                                    SeparatedSyntaxList<BaseExpressionSyntax> arguments,
                                    SyntaxToken closeParenthesisToken)
            : base(syntaxTree)
        {
            Identifier = identifier;
            OpenParenthesisToken = openParenthesisToken;
            Arguments = arguments;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public SyntaxToken Identifier { get; }
        public SyntaxToken OpenParenthesisToken { get; }
        public SeparatedSyntaxList<BaseExpressionSyntax> Arguments { get; }
        public SyntaxToken CloseParenthesisToken { get; }
        public override SyntaxKind Kind => SyntaxKind.CallExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Identifier;
            yield return OpenParenthesisToken;
            foreach (var child in Arguments.GetWithSeparators())
                yield return child;
            yield return CloseParenthesisToken;
        }
    }
}