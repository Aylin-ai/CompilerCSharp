using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(SyntaxToken keyword, SyntaxToken identifier, 
        SyntaxToken equalsToken, BaseExpressionSyntax initializer){
            Keyword = keyword;
            Identifier = identifier;
            EqualsToken = equalsToken;
            Initializer = initializer;
        }
        public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;

        public SyntaxToken Keyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public BaseExpressionSyntax Initializer { get; }
    }
}