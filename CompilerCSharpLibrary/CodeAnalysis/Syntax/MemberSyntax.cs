namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public abstract class MemberSyntax : SyntaxNode
    {
        protected MemberSyntax(SyntaxTree syntaxTree)
            : base(syntaxTree)
        {
        }
    }
}