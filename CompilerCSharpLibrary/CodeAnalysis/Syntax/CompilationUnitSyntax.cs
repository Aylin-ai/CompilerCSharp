using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode{
        public CompilationUnitSyntax(BaseExpressionSyntax expression, SyntaxToken endOfFileToken){
            Expression = expression;
            EndOfFileToken = endOfFileToken;
        }

        public BaseExpressionSyntax Expression { get; }
        public SyntaxToken EndOfFileToken { get; }

        public override SyntaxKind Kind => SyntaxKind.CompilationUnit;
    }
}