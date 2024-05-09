using System.Collections.Generic;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        public ForStatementSyntax(SyntaxTree syntaxTree, SyntaxToken forKeyword, SyntaxToken identifier,
        SyntaxToken equalToken, BaseExpressionSyntax lowerBound, SyntaxToken toKeyword,
        BaseExpressionSyntax upperBound, StatementSyntax body)
            : base(syntaxTree)
        {
            ForKeyword = forKeyword;
            Identifier = identifier;
            EqualsToken = equalToken;
            LowerBound = lowerBound;
            ToKeyword = toKeyword;
            UpperBound = upperBound;
            Body = body;
        }

        public SyntaxToken ForKeyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public BaseExpressionSyntax LowerBound { get; }
        public SyntaxToken ToKeyword { get; }
        public BaseExpressionSyntax UpperBound { get; }
        public StatementSyntax Body { get; }

        public override SyntaxKind Kind => SyntaxKind.ForStatement;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return ForKeyword;
            yield return Identifier;
            yield return EqualsToken;
            yield return LowerBound;
            yield return ToKeyword;
            yield return UpperBound;
            yield return Body;
        }
    }
}