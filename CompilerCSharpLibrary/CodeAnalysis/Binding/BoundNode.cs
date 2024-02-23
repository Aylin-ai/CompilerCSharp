using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    /*
    Абстрактный класс, отражающий узел в АСД. Содержит
    в себе вид узла.
    */
    public abstract class BoundNode{
        public abstract BoundNodeKind Kind { get; }
    }
}