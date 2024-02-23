using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public sealed class BlockStatementSyntax : StatementSyntax{
        public BlockStatementSyntax(SyntaxToken openBraceToken, List<StatementSyntax> statements, SyntaxToken closeBraceToken){
            OpenBraceToken = openBraceToken;
            Statements = statements;
            CloseBraceToken = closeBraceToken;
        }

        public SyntaxToken OpenBraceToken { get; }
        public List<StatementSyntax> Statements { get; }
        public SyntaxToken CloseBraceToken { get; }

        public override SyntaxKind Kind => SyntaxKind.BlockStatement;
    }
}