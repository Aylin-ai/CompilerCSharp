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

using System.Collections.Generic;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions
{
    public sealed class BoundCallExpression : BoundExpression{
        public BoundCallExpression(FunctionSymbol function, List<BoundExpression> arguments){
            Function = function;
            Arguments = arguments;
        }

        public override TypeSymbol Type => Function.Type;

        public override BoundNodeKind Kind => BoundNodeKind.CallExpression;

        public FunctionSymbol Function { get; }
        public List<BoundExpression> Arguments { get; }
    }
}