namespace CompilerCSharp.CodeAnalysis.Binding
{
    /*
    Абстрактный класс, наследуемый от абстрактного класса
    BoundNode. Добавляет к виду узла в АСД тип выражения,
    которое содержит в себе.
    */
    internal abstract class BoundExpression : BoundNode{
        public abstract Type Type { get; }
    }
}