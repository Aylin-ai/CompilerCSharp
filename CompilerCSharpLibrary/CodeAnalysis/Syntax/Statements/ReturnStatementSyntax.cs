using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements
{
    public sealed class ReturnStatementSyntax : StatementSyntax{
        public ReturnStatementSyntax(SyntaxToken returnKeyword, BaseExpressionSyntax expression){
            ReturnKeyword = returnKeyword;
            Expression = expression;
        }

        public SyntaxToken ReturnKeyword { get; }
        public BaseExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.ReturnStatement;
    }
}