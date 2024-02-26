using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundOperators;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Lowering{
    public sealed class Lowerer : BoundTreeRewriter{
        private int _labelCount;
        private Lowerer(){

        }

        private BoundLabel GenerateLabel(){
            var name = $"Label{++_labelCount}";
            return new BoundLabel(name);
        }

        public static BoundBlockStatement Lower(BoundStatement statement){
            var lowerer = new Lowerer();
            var result = lowerer.RewriteStatement(statement);   
            return Flatten(result); 
        }

        private static BoundBlockStatement Flatten(BoundStatement statement){
            var statements = new List<BoundStatement>();
            var stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while (stack.Count > 0){
                var current = stack.Pop();

                if (current is BoundBlockStatement block){
                    block.Statements.Reverse();
                    foreach (var s in block.Statements){
                        stack.Push(s);
                    }
                }
                else{
                    statements.Add(current);
                }
            }

            return new BoundBlockStatement(statements);
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.ElseStatement == null){
                /*
                    if <condition>
                        <then>

                    ----->

                    gotoIfFalse <condition> end
                    <then>
                    end:
                */
                var endLabel = GenerateLabel();
                var gotoFalse = new BoundConditionalGotoStatement(endLabel, node.Condition, false);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(
                    new List<BoundStatement>(){
                        gotoFalse, node.ThenStatement, endLabelStatement
                    }
                );
                return RewriteStatement(result);
            }
            else{
                /*
                    if <condition>
                        <then>
                    else
                        <else>

                    ----->

                    gotoIfFalse <condition> else
                    <then>
                    goto end
                    else:
                    <else>
                    end:
                */
                var elseLabel = GenerateLabel();
                var endLabel = GenerateLabel();
                var gotoFalse = new BoundConditionalGotoStatement(elseLabel, node.Condition, false);
                var gotoEndStatement = new BoundGotoStatement(endLabel);
                var elseLabelStatement = new BoundLabelStatement(elseLabel);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(
                    new List<BoundStatement>(){
                        gotoFalse, node.ThenStatement, gotoEndStatement, 
                        elseLabelStatement, node.ElseStatement, endLabelStatement
                    }
                );
                return RewriteStatement(result);
            }
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            /*
                while <condition>
                    <body>

                ------->

                goto check
                continue:
                <body>
                check:
                gotoTrue <condition> continue
                end:
            */

            var continueLabel = GenerateLabel();
            var endLabel = GenerateLabel();
            var checkLabel = GenerateLabel();
            
            var gotoCheck = new BoundGotoStatement(checkLabel);
            var continueLabelStatement = new BoundLabelStatement(continueLabel);
            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var gotoTrue = new BoundConditionalGotoStatement(continueLabel, node.Condition, true);
            var endLabelStatement = new BoundLabelStatement(endLabel);

            var result = new BoundBlockStatement(
                new List<BoundStatement>(){
                    gotoCheck, continueLabelStatement, node.Body, 
                    checkLabelStatement, gotoTrue, endLabelStatement
                }
            );
            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            /*
                for <var> = <lower> to <upper>
                    <body>

                ----->

                {
                    var <var> = <lower>
                    let upperBound = <upper>
                    while (<var> <= upperBound){
                        <body>
                        <var> = <var> + 1
                    }
                }
            */

            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound);
            var variableExpression = new BoundVariableExpression(node.Variable);
            var upperBoundSymbol = new VariableSymbol("upperBound", true, typeof(int));
            var upperBoundDeclaration = new BoundVariableDeclaration(upperBoundSymbol, node.UpperBound);
            var condition = new BoundBinaryExpression(
                variableExpression, 
                BoundBinaryOperator.Bind(SyntaxKind.LessOrEqualsToken, typeof(int), typeof(int)),
                new BoundVariableExpression(upperBoundSymbol)
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
                    variableDeclaration, upperBoundDeclaration, whileStatement
                }
            );

            return RewriteStatement(result);
        }
    }
}