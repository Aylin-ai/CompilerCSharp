using System.Runtime.InteropServices;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundOperators;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;

/*
Абстрактное синтаксическое дерево требуется для того, чтобы
синтаксическое дерево оставалось неизменяемым.
АСД(AST) является разделением задач.
Синтаксические классы показывают синтаксис языка программирования
наиболее четко чтобы отлавливать ошибки.
АСД нужны для отображения внутреннего состояния компилятора.
Разделение просто упрощает понимание и написание кода.
В данном случае в АСД происходит работа с типами выражений, литерал
и операторов. Мы рассматриваем классы Binding больше с абстрактной точки 
зрения, чем с конкретной, в отличие от классов Syntax (Пример, оператор +. 
В Binding на него есть более подробная информация, когда как в Syntax
все ограничивается его токеном)
АСД нужно, чтобы хранить больше информации, в отличие от
синтаксического дерева, а также чтобы последнее было неизменяемо
*/
namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    public sealed class Binder{
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private BoundScope _scope;

        public Binder(BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax){
            var parentScope = CreateParentScopes(previous);
            var binder = new Binder(parentScope);
            var expression = binder.BindExpression(syntax.Expression);
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics;

            if (previous != null){
                diagnostics.AddRange(previous.Diagnostics);
            }

            return new BoundGlobalScope(previous, diagnostics, variables, expression);
        }

        private static BoundScope CreateParentScopes(BoundGlobalScope previous){
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null){
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope parent = null;

            while (stack.Count > 0){
                previous = stack.Pop();
                var scope = new BoundScope(parent);
                foreach (var v in previous.Variables){
                    scope.TryDeclare(v);
                }

                parent = scope;
            }

            return parent;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        //Возвращает нужный вид выражение с приведением его параметра к нужному типу
        public BoundExpression BindExpression(BaseExpressionSyntax syntax){
            switch (syntax.Kind){
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.ParethesizedExpression:
                    return BindParethesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);

                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }
        /*
        Метод, срабатывающий при приравнивании переменной к значению. Получает
        имя переменной, рассматривает выражение. Если такая переменная уже
        существует, то удаляет ее из списка и пересоздает по новой. Если
        такой переменной нет, то просто создает её. В конце возвращает
        узел с информацией по переменной и выражению BoundAssignmentExpression
        */
        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if (!_scope.TryLookup(name, out var variable)){
                variable = new VariableSymbol(name, boundExpression.Type);
                _scope.TryDeclare(variable);
            }

            if (boundExpression.Type != variable.Type){
                _diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
                return boundExpression;
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }
        /*
        Метод, срабатывающий при вызове переменной. Получает ее имя и ищет среди
        всех переменных. Если находит, то возвращает узел с информацией
        по этой переменной BoundVariableExpression. Если нет, то ловит ошибку
        и возвращает узел с литералом, равным 0
        */
        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            
            if (!_scope.TryLookup(name, out var variable)){
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }

            return new BoundVariableExpression(variable);
        }

        //Реализация выражений в скобках
        private BoundExpression BindParethesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        /*
        Возвращает объект класса BindBinaryExpression,
        данные для которого получены из синтаксического 
        поддерева BinaryExpressionSyntax
        */
        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            BoundExpression boundLeft = BindExpression(syntax.Left);
            BoundExpression boundRight = BindExpression(syntax.Right);
            BoundBinaryOperator boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            if (boundOperator == null){
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return boundLeft;
            }
            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }

        /*
        Возвращает объект класса BoundUnaryExpression,
        данные для которого получены из синтаксического 
        поддерева UnaryExpressionSyntax
        */
        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            BoundExpression boundOperand = BindExpression(syntax.Operand);
            BoundUnaryOperator boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
            if (boundOperator == null){
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        /*
        Возвращает объект класса BoundLiteralExpression,
        содержащий значение, его тип и вид выражения,
        полученные из синтаксического поддерева LiteralExpressionSyntax
        */
        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            object value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }
    }
}