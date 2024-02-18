using CompilerCSharpLibrary.CodeAnalysis.Text;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий токен и все его атрибуты: тип, позиция в тексте,
    лексема и значение, если есть. У данного токена нет дочерних токенов
    */
    public class SyntaxToken : SyntaxNode{
        public SyntaxToken(SyntaxKind kind, int position, string text, object value){
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; }

        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
        public override TextSpan Span => new TextSpan(Position, Text.Length);
    }
}