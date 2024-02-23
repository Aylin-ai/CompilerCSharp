using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{
    /*
    Класс, представляющий узел, от которого исходит 2 узла,
    являющиеся оператором и операндом унарного выражения.
    Операнд может ветвиться дальше, то есть
    сам являться деревом, т.к. класса ExpressionSyntax.
    */
    public sealed class UnaryExpressionSyntax : BaseExpressionSyntax{
        public UnaryExpressionSyntax(SyntaxToken operatorToken, BaseExpressionSyntax operand){
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public SyntaxToken OperatorToken { get; }
        public BaseExpressionSyntax Operand { get; }
    }
}