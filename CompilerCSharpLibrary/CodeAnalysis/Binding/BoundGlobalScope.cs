using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;

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
    public sealed class BoundGlobalScope{
        public BoundGlobalScope(BoundGlobalScope previous, DiagnosticBag diagnostics, 
        List<VariableSymbol> variables, BoundExpression expression){
            Previous = previous;
            Diagnostics = diagnostics;
            Variables = variables;
            Expression = expression;
        }

        public BoundGlobalScope Previous { get; }
        public DiagnosticBag Diagnostics { get; }
        public List<VariableSymbol> Variables { get; }
        public BoundExpression Expression { get; }
    }
}