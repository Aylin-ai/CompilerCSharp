using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements
{
    public sealed class ElseClauseSyntax : SyntaxNode{
        public ElseClauseSyntax(SyntaxToken elseKeyword, StatementSyntax elseStatement){
            ElseKeyword = elseKeyword;
            ElseStatement = elseStatement;
        }

        public SyntaxToken ElseKeyword { get; }
        public StatementSyntax ElseStatement { get; }

        public override SyntaxKind Kind => SyntaxKind.ElseClause;
    }

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