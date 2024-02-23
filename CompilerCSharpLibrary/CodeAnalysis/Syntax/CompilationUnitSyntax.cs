using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode{
        public CompilationUnitSyntax(StatementSyntax statement, SyntaxToken endOfFileToken){
            Statement = statement;
            EndOfFileToken = endOfFileToken;
        }

        public StatementSyntax Statement { get; }
        public SyntaxToken EndOfFileToken { get; }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
    }
}