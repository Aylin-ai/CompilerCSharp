using System;
using System.Collections.Generic;
using System.Linq;
using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundOperators;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Lowering
{
    public sealed class Lowerer : BoundTreeRewriter
    {

        #region Поля класса

        private int _labelCount;

        #endregion

        #region Конструкторы класса

        private Lowerer()
        {

        }

        #endregion

        #region Методы класса

        private BoundLabel GenerateLabel()
        {
            string? name = $"Label{++_labelCount}";
            return new BoundLabel(name);
        }

        public static BoundBlockStatement Lower(FunctionSymbol function, BoundStatement statement)
        {
            Lowerer? lowerer = new Lowerer();
            BoundStatement? result = lowerer.RewriteStatement(statement);
            return RemoveDeadCode(Flatten(function, result));
        }

        private static BoundBlockStatement Flatten(FunctionSymbol function, BoundStatement statement)
        {
            List<BoundStatement>? builder = new List<BoundStatement>();
            Stack<BoundStatement>? stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while (stack.Count > 0)
            {
                BoundStatement? current = stack.Pop();

                if (current is BoundBlockStatement block)
                {
                    block.Statements.Reverse();
                    foreach (BoundStatement? s in block.Statements)
                        stack.Push(s);
                }
                else
                {
                    builder.Add(current);
                }
            }

            if (function.Type == TypeSymbol.Void)
            {
                if (builder.Count == 0 || CanFallTrough(builder.Last()))
                {
                    builder.Add(new BoundReturnStatement(null));
                }
            }

            return new BoundBlockStatement(builder);
        }

        private static BoundBlockStatement RemoveDeadCode(BoundBlockStatement node)
        {
            var controlFlow = ControlFlowGraph.Create(node);
            var reachableStatements = new HashSet<BoundStatement>(
                controlFlow.Blocks.SelectMany(b => b.Statements));

            var builder = node.Statements;
            for (int i = builder.Count - 1; i >= 0; i--)
            {
                if (!reachableStatements.Contains(builder[i]))
                    builder.RemoveAt(i);
            }

            return new BoundBlockStatement(builder);
        }

        private static bool CanFallTrough(BoundStatement boundStatement)
        {
            return boundStatement.Kind != BoundNodeKind.ReturnStatement &&
                   boundStatement.Kind != BoundNodeKind.GotoStatement;
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.ElseStatement == null)
            {
                /*
                    if <condition>
                        <then>

                    ----->

                    gotoIfFalse <condition> end
                    <then>
                    end:
                */
                BoundLabel? endLabel = GenerateLabel();
                BoundConditionalGotoStatement? gotoFalse = new BoundConditionalGotoStatement(endLabel, node.Condition, false);
                BoundLabelStatement? endLabelStatement = new BoundLabelStatement(endLabel);
                BoundBlockStatement? result = new BoundBlockStatement(
                    new List<BoundStatement>(){
                        gotoFalse, node.ThenStatement, endLabelStatement
                    }
                );
                return RewriteStatement(result);
            }
            else
            {
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
                BoundLabel? elseLabel = GenerateLabel();
                BoundLabel? endLabel = GenerateLabel();
                BoundConditionalGotoStatement? gotoFalse = new BoundConditionalGotoStatement(elseLabel, node.Condition, false);
                BoundGotoStatement? gotoEndStatement = new BoundGotoStatement(endLabel);
                BoundLabelStatement? elseLabelStatement = new BoundLabelStatement(elseLabel);
                BoundLabelStatement? endLabelStatement = new BoundLabelStatement(endLabel);
                BoundBlockStatement? result = new BoundBlockStatement(
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
                break:
            */

            BoundLabel? checkLabel = GenerateLabel();

            BoundGotoStatement? gotoCheck = new BoundGotoStatement(checkLabel);
            BoundLabelStatement? continueLabelStatement = new BoundLabelStatement(node.ContinueLabel);
            BoundLabelStatement? checkLabelStatement = new BoundLabelStatement(checkLabel);
            BoundConditionalGotoStatement? gotoTrue = new BoundConditionalGotoStatement(node.ContinueLabel, node.Condition, true);
            BoundLabelStatement? breakLabelStatement = new BoundLabelStatement(node.BreakLabel);

            BoundBlockStatement? result = new BoundBlockStatement(
                new List<BoundStatement>(){
                    gotoCheck, continueLabelStatement, node.Body,
                    checkLabelStatement, gotoTrue, breakLabelStatement
                }
            );
            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteDoWhileStatement(BoundDoWhileStatement node)
        {
            /*
                do
                    <body>
                while <condition>

                ------->

                continue:
                <body>
                gotoTrue <condition> continue
                break:
            */

            BoundLabelStatement? continueLabelStatement = new BoundLabelStatement(node.ContinueLabel);
            BoundConditionalGotoStatement? gotoTrue = new BoundConditionalGotoStatement(node.ContinueLabel, node.Condition, true);
            BoundLabelStatement? breakLabelStatement = new BoundLabelStatement(node.BreakLabel);

            BoundBlockStatement? result = new BoundBlockStatement(
                new List<BoundStatement>(){
                    continueLabelStatement, node.Body, gotoTrue, breakLabelStatement
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
                        continue:
                        <var> = <var> + 1
                    }
                }
            */

            BoundVariableDeclaration? variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound);
            BoundVariableExpression? variableExpression = new BoundVariableExpression(node.Variable);
            LocalVariableSymbol? upperBoundSymbol = new LocalVariableSymbol("upperBound", true, TypeSymbol.Int, node.UpperBound.ConstantValue);
            BoundVariableDeclaration? upperBoundDeclaration = new BoundVariableDeclaration(upperBoundSymbol, node.UpperBound);
            BoundBinaryExpression? condition = new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxKind.LessOrEqualsToken, TypeSymbol.Int, TypeSymbol.Int),
                new BoundVariableExpression(upperBoundSymbol)
            );

            BoundLabelStatement? continueLabelStatement = new BoundLabelStatement(node.ContinueLabel);

            BoundExpressionStatement? increment = new BoundExpressionStatement(
                new BoundAssignmentExpression(
                    node.Variable,
                    new BoundBinaryExpression(
                        variableExpression,
                        BoundBinaryOperator.Bind(SyntaxKind.PlusToken, TypeSymbol.Int, TypeSymbol.Int),
                        new BoundLiteralExpression(1)
                    )
                )
            );

            BoundBlockStatement? whileBody = new BoundBlockStatement(
                new List<BoundStatement>(){
                    node.Body, continueLabelStatement, increment
                }
            );

            BoundWhileStatement? whileStatement = new BoundWhileStatement(condition, whileBody, node.BreakLabel, GenerateLabel());
            BoundBlockStatement? result = new BoundBlockStatement(
                new List<BoundStatement>(){
                    variableDeclaration, upperBoundDeclaration, whileStatement
                }
            );

            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteConditionalGotoStatement(BoundConditionalGotoStatement node)
        {
            if (node.Condition.ConstantValue != null)
            {
                var condition = (bool)node.Condition.ConstantValue.Value;
                condition = node.JumpIfTrue ? condition : !condition;
                if (condition)
                    return RewriteStatement(new BoundGotoStatement(node.Label));
                else
                    return RewriteStatement(new BoundNopStatement());
            }

            return base.RewriteConditionalGotoStatement(node);
        }

        #endregion

    }
}