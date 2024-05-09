using System.Linq.Expressions;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using System.Collections.Generic;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{

    /*
    Класс, представляющий узел, от которого исходит 3 узла,
    являющиеся левым операндом, оператором и правым операндом
    бинарного выражения.
    Левые и правые операнды могут ветвиться дальше, то есть
    сами являться деревьями, т.к. класса ExpressionSyntax.
    */
    public sealed class BinaryExpressionSyntax : BaseExpressionSyntax{
        public BinaryExpressionSyntax(SyntaxTree syntaxTree, BaseExpressionSyntax left, SyntaxToken operatorToken, BaseExpressionSyntax right)
            : base(syntaxTree)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public BaseExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public BaseExpressionSyntax Right { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}