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
    /*
    Класс, представляющий узел приравнивания значения к переменной
    в АСД. Содержит в себе информацию по переменной и выражении,
    а также тип выражения и вид самого узла
    */
    public sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }

        public override Type Type => Expression.Type;

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;

        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
    }
}