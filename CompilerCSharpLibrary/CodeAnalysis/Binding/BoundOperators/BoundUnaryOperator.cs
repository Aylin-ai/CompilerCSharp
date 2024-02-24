using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.BoundOperators
{
    /*
    Класс, описывающий унарный оператор в АСД. Содержит в себе информацию
    о синтаксическом типе оператора(его токен), тип его оператора,
    тип операнда, к которому приставлен оператор и результирующий тип,
    который должен получиться в результате выполнения оператора
    */
    public sealed class BoundUnaryOperator{
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType)
        : this(syntaxKind, kind, operandType, operandType) { }
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, Type operandType, Type type){
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandType = operandType;
            Type = type;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public Type OperandType { get; }
        public Type Type { get; }

        //Список доступных операторов
        private static BoundUnaryOperator[] _operators = {
            new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
            
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, typeof(int)),
            new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesComplement, typeof(int))
        };

        //Возвращает нужный оператор из списка на основании переданных параметров
        public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, Type operandType){
            foreach (var op in _operators){
                if (op.SyntaxKind == syntaxKind && op.OperandType == operandType)
                    return op;
            }

            return null;
        }
    }
}