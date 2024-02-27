using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode{
        public CompilationUnitSyntax(List<MemberSyntax> members, SyntaxToken endOfFileToken){
            Members = members;
            EndOfFileToken = endOfFileToken;
        }

        public List<MemberSyntax> Members { get; }
        public SyntaxToken EndOfFileToken { get; }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
    }
}