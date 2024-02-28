namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base
{
    public abstract class StatementSyntax : SyntaxNode
    {
        protected StatementSyntax(SyntaxTree syntaxTree) 
        : base(syntaxTree)
        {
        }
    }
}