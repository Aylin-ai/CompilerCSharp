using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundOperators;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Lowering{
    public sealed class Lowerer : BoundTreeRewriter{
        private Lowerer(){

        }

        public static BoundStatement Lower(BoundStatement statement){
            var lowerer = new Lowerer();
            return lowerer.RewriteStatement(statement);    
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            /*
                for <var> = <lower> to <upper>
                    <body>

                ----->

                {
                    var <var> = <lower>
                    while (<var> <= <upper>){
                        <body>
                        <var> = <var> + 1
                    }
                }
            */

            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound);
            var variableExpression = new BoundVariableExpression(node.Variable);
            var condition = new BoundBinaryExpression(
                variableExpression, 
                BoundBinaryOperator.Bind(SyntaxKind.LessOrEqualsToken, typeof(int), typeof(int)),
                node.UpperBound
            );

            var increment = new BoundExpressionStatement(
                new BoundAssignmentExpression(
                    node.Variable,
                    new BoundBinaryExpression(
                        variableExpression,
                        BoundBinaryOperator.Bind(SyntaxKind.PlusToken, typeof(int), typeof(int)),
                        new BoundLiteralExpression(1)
                    )
                )
            );

            var whileBody = new BoundBlockStatement(
                new List<BoundStatement>(){
                    node.Body, increment
                }
            );

            var whileStatement = new BoundWhileStatement(condition, whileBody);
            var result = new BoundBlockStatement(
                new List<BoundStatement>(){
                    variableDeclaration, whileStatement
                }
            );

            return RewriteStatement(result);
        }
    }
}