using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public sealed class GlobalStatementSyntax : MemberSyntax{
        public GlobalStatementSyntax(StatementSyntax statement){
            Statement = statement;
        }

        public StatementSyntax Statement { get; }

        public override SyntaxKind Kind => SyntaxKind.GlobalStatement;
    }
}