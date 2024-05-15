using System.Linq.Expressions;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using System.Collections.Generic;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{
    public sealed class BinaryExpressionSyntax : BaseExpressionSyntax
    {
        public BinaryExpressionSyntax(SyntaxTree syntaxTree,
                                      BaseExpressionSyntax left,
                                      SyntaxToken operatorToken,
                                      BaseExpressionSyntax right)
            : base(syntaxTree)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }


        public BaseExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public BaseExpressionSyntax Right { get; }
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}