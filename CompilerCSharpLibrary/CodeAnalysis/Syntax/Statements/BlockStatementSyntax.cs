using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements
{
    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(SyntaxTree syntaxTree, SyntaxToken openBraceToken,
        List<StatementSyntax> statements, SyntaxToken closeBraceToken)
            : base(syntaxTree)
        {
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