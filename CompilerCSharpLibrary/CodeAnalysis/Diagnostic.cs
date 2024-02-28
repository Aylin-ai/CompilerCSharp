using CompilerCSharpLibrary.CodeAnalysis.Text;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, представляющий собой пойманную ошибку.
    Содержит в себе информацию об ошибке в тексте
    (TextSpan) и текст ошибки
    */
    public sealed class Diagnostic{
        public Diagnostic(TextLocation location, string message){
            Location = location;
            Message = message;
        }

        public TextLocation Location { get; }
        public string Message { get; }

        public override string ToString() => Message;

    }
}