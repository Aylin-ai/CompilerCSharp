using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements
{
    public sealed class IfStatementSyntax : StatementSyntax{
        public IfStatementSyntax(SyntaxToken ifKeyword, BaseExpressionSyntax condition, 
        StatementSyntax thenStatement, ElseClauseSyntax elseClause){
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenStatement = thenStatement;
            ElseClause = elseClause;
        }

        public SyntaxToken IfKeyword { get; }
        public BaseExpressionSyntax Condition { get; }
        public StatementSyntax ThenStatement { get; }
        public ElseClauseSyntax ElseClause { get; }

        public override SyntaxKind Kind => SyntaxKind.IfStatement;
    }
}