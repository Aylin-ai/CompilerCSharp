using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements
{
    public sealed class WhileStatementSyntax : StatementSyntax{
        public WhileStatementSyntax(SyntaxToken whileKeyword, BaseExpressionSyntax condition,
        StatementSyntax body){
            WhileKeyword = whileKeyword;
            Condition = condition;
            Body = body;
        }

        public SyntaxToken WhileKeyword { get; }
        public BaseExpressionSyntax Condition { get; }
        public StatementSyntax Body { get; }

        public override SyntaxKind Kind => SyntaxKind.WhileStatement;
    }
}