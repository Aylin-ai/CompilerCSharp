using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;
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
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType)
        : this(syntaxKind, kind, operandType, operandType) { }
        private BoundUnaryOperator(SyntaxKind syntaxKind, BoundUnaryOperatorKind kind, TypeSymbol operandType, TypeSymbol type){
            SyntaxKind = syntaxKind;
            Kind = kind;
            OperandType = operandType;
            Type = type;
        }

        public SyntaxKind SyntaxKind { get; }
        public BoundUnaryOperatorKind Kind { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol Type { get; }

        //Список доступных операторов
        private static BoundUnaryOperator[] _operators = {
            new BoundUnaryOperator(SyntaxKind.BangToken, BoundUnaryOperatorKind.LogicalNegation, TypeSymbol.Bool),
            
            new BoundUnaryOperator(SyntaxKind.PlusToken, BoundUnaryOperatorKind.Identity, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxKind.MinusToken, BoundUnaryOperatorKind.Negation, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxKind.TildeToken, BoundUnaryOperatorKind.OnesComplement, TypeSymbol.Int)
        };

        //Возвращает нужный оператор из списка на основании переданных параметров
        public static BoundUnaryOperator Bind(SyntaxKind syntaxKind, TypeSymbol operandType){
            foreach (BoundUnaryOperator? op in _operators){
                if (op.SyntaxKind == syntaxKind && op.OperandType == operandType)
                    return op;
            }

            return null;
        }
    }
}