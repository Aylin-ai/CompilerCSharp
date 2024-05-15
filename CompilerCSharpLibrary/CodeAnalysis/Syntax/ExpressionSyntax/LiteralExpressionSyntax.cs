using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using System.Collections.Generic;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{
    public sealed class LiteralExpressionSyntax : BaseExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxTree syntaxTree,
                                       SyntaxToken literalToken,
                                       object value)
            : base(syntaxTree)
        {
            LiteralToken = literalToken;
            Value = value;
        }
        public LiteralExpressionSyntax(SyntaxTree syntaxTree,
                                       SyntaxToken literalToken)
            : this(syntaxTree, literalToken, literalToken.Value) { }

        public SyntaxToken LiteralToken { get; }
        public object Value { get; }
        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken;
        }
    }
}