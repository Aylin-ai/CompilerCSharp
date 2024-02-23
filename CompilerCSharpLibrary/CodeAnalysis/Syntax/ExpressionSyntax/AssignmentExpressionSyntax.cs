using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{
    /*
    Класс, представляющий выражение присваивания переменной значения.
    Содержит в себе токены переменной, = и само выражение, результат которого
    присваивается переменной.
    */
    public sealed class AssignmentExpressionSyntax : BaseExpressionSyntax{
        public AssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken equalsToken, BaseExpressionSyntax expression){
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            Expression = expression;
        }

        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualsToken { get; }
        public BaseExpressionSyntax Expression { get; }

        public override SyntaxKind Kind => SyntaxKind.AssignmentExpression;
    }
}