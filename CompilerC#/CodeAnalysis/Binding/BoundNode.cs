namespace CompilerCSharp.CodeAnalysis.Binding
{
    /*
    Абстрактный класс, отражающий узел в АСД. Содержит
    в себе вид узла.
    */
    internal abstract class BoundNode{
        public abstract BoundNodeKind Kind { get; }
    }
}