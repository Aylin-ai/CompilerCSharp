using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public sealed class ParameterSyntax : SyntaxNode
    {
        public ParameterSyntax(SyntaxTree syntaxTree, SyntaxToken identifier, TypeClauseSyntax type)
            : base(syntaxTree)
        {
            Identifier = identifier;
            Type = type;
        }
        public override SyntaxKind Kind => SyntaxKind.Parameter;

        public SyntaxToken Identifier { get; }
        public TypeClauseSyntax Type { get; }
    }
}