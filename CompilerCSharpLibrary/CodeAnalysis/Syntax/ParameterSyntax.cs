using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public sealed class ParameterSyntax : SyntaxNode
    {
        public ParameterSyntax(SyntaxToken identifier, TypeClauseSyntax type){
            Identifier = identifier;
            Type = type;
        }
        public override SyntaxKind Kind => SyntaxKind.Parameter;

        public SyntaxToken Identifier { get; }
        public TypeClauseSyntax Type { get; }
    }
}