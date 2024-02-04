namespace CompilerCSharp.CodeAnalysis
{
    /*
    Абстрактный класс, представляющий узел в синтаксическом дереве.
    От него исходят все остальные классы, включая SyntaxToken.
    Содержит тип токена и метод получения дочерних токенов.
    */
    abstract class SyntaxNode{
        public abstract SyntaxKind Kind { get;}

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}