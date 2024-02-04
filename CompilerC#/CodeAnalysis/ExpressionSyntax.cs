namespace CompilerCSharp.CodeAnalysis
{
    /*
    Абстрактный класс, представляющий узел выражения,
    от которого идет реализация конкретных узлов выражения, по типу
    NumberExpression или BinaryExpression
    */
    abstract class ExpressionSyntax : SyntaxNode{

    }
}