using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{
    /*
    Класс, представляющий узел числа, от которого идет
    узел, представляющий числовой токен.
    В качестве дочерних узлов содержит один узел,
    представляющий число
    */
    public sealed class LiteralExpressionSyntax : BaseExpressionSyntax{
        public LiteralExpressionSyntax(SyntaxToken literalToken, object value){
            LiteralToken = literalToken;
            Value = value;
        }
        public LiteralExpressionSyntax(SyntaxToken literalToken) : this(literalToken, literalToken.Value) { }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken LiteralToken { get; }
        public object Value { get; }
    }
}