using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public sealed class ExpressionStatementSyntax : StatementSyntax{
        public ExpressionStatementSyntax(BaseExpressionSyntax expression){
            Expression = expression;
        }

        public BaseExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.ExpressionStatement;
    }
}