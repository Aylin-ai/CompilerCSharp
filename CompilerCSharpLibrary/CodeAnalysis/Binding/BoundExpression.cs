namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    /*
    Абстрактный класс, наследуемый от абстрактного класса
    BoundNode. Добавляет к виду узла в АСД тип выражения,
    которое содержит в себе.
    */
    public abstract class BoundExpression : BoundNode{
        public abstract Type Type { get; }
    }
}