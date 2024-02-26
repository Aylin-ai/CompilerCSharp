using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements
{
    public sealed class DoWhileStatementSyntax : StatementSyntax{
        public DoWhileStatementSyntax(SyntaxToken doKeyword, StatementSyntax body, 
        SyntaxToken whileKeyword, BaseExpressionSyntax condition
        ){
            DoKeyword = doKeyword;
            Body = body;
            WhileKeyword = whileKeyword;
            Condition = condition;
        }

        public SyntaxToken WhileKeyword { get; }
        public BaseExpressionSyntax Condition { get; }
        public SyntaxToken DoKeyword { get; }
        public StatementSyntax Body { get; }

        public override SyntaxKind Kind => SyntaxKind.DoWhileStatement;
    }
}