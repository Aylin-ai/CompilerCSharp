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
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    public sealed class Conversion
    {
        public static readonly Conversion None = new Conversion(exist: false, isIdentity: false, isImplicit: false);
        //Identity - типы равны
        public static readonly Conversion Identity = new Conversion(exist: true, isIdentity: true, isImplicit: true);
        //При преобразовании информация теряется (float -> int)
        public static readonly Conversion Implicit = new Conversion(exist: true, isIdentity: false, isImplicit: true);
        //При преобразовании информация не теряется (int -> float)
        public static readonly Conversion Explicit = new Conversion(exist: true, isIdentity: false, isImplicit: false);
        private Conversion(bool exist, bool isIdentity, bool isImplicit)
        {
            Exist = exist;
            IsIdentity = isIdentity;
            IsImplicit = isImplicit;
        }

        public bool Exist { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }
        public bool IsExplicit => Exist && !IsImplicit;

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
                return Identity;

            if (from == TypeSymbol.Int || from == TypeSymbol.Bool)
            {
                if (to == TypeSymbol.String)
                {
                    return Explicit;
                }
            }
            else if (from == TypeSymbol.String){
                if (to == TypeSymbol.Int || to == TypeSymbol.Bool)
                    return Explicit;
            }

            return None;
        }

    }
}