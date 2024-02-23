using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements
{
    public sealed class ForStatementSyntax : StatementSyntax{
        public ForStatementSyntax(SyntaxToken forKeyword, SyntaxToken identifier,
        SyntaxToken equalToken, BaseExpressionSyntax lowerBound, SyntaxToken toKeyword,
         BaseExpressionSyntax upperBound, StatementSyntax body){
            ForKeyword = forKeyword;
            Identifier = identifier;
            EqualToken = equalToken;
            LowerBound = lowerBound;
            ToKeyword = toKeyword;
            UpperBound = upperBound;
            Body = body;
        }

        public SyntaxToken ForKeyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualToken { get; }
        public BaseExpressionSyntax LowerBound { get; }
        public SyntaxToken ToKeyword { get; }
        public BaseExpressionSyntax UpperBound { get; }
        public StatementSyntax Body { get; }

        public override SyntaxKind Kind => SyntaxKind.ForStatement;
    }
}