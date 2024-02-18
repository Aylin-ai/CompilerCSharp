using System.Reflection;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Абстрактный класс, представляющий узел в синтаксическом дереве.
    От него исходят все остальные классы, включая SyntaxToken.
    Содержит тип токена и метод получения дочерних токенов.
    */
    public abstract class SyntaxNode{
        public abstract SyntaxKind Kind { get;}

        public IEnumerable<SyntaxNode> GetChildren(){
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties){
                //Можно ли преобразовать property.PropertyType в SyntaxNode?
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType)){
                    var child = (SyntaxNode)property.GetValue(this);
                    yield return child;
                }
                //Можно ли преобразовать property.PropertyType в IEnumerable<SyntaxNode>?
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType)){
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this);
                    foreach (var child in children)
                        yield return child;
                }
            }
        }
    }
}