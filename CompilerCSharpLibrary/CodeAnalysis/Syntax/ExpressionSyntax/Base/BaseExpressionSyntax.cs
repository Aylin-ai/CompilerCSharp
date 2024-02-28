using CompilerCSharpLibrary.CodeAnalysis.Syntax;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base
{
    /*
    Абстрактный класс, представляющий узел выражения,
    от которого идет реализация конкретных узлов выражения, по типу
    NumberExpression или BinaryExpression
    */
    public abstract class BaseExpressionSyntax : SyntaxNode
    {
        protected BaseExpressionSyntax(SyntaxTree syntaxTree) 
            : base(syntaxTree)
        {
        }
    }
}