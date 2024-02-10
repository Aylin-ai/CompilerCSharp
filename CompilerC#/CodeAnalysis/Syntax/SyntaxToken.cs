namespace CompilerCSharp.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий токен и все его атрибуты: тип, позиция в тексте,
    лексема и значение, если есть. У данного токена нет дочерних токенов
    */
    class SyntaxToken : SyntaxNode{
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

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}