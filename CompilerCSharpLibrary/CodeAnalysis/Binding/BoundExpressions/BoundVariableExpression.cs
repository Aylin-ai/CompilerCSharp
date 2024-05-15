using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions
{
    /*
    Класс, представляющий узел с переменной в АСД.
    Содержит в себе информацию по переменной - имя,тип
    */
    public sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override TypeSymbol Type => Variable.Type;
        public VariableSymbol Variable { get; }
        public override BoundConstant ConstantValue => Variable.Constant;
        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
    }
}