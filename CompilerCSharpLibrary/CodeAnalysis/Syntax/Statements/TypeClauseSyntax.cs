using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements
{
    public sealed class TypeClauseSyntax : SyntaxNode{
        public TypeClauseSyntax(SyntaxToken colonToken, SyntaxToken identifier){
            ColonToken = colonToken;
            Identifier = identifier;
        }

        public SyntaxToken ColonToken { get; }
        public SyntaxToken Identifier { get; }

        public override SyntaxKind Kind => SyntaxKind.TypeClause;
    }
}