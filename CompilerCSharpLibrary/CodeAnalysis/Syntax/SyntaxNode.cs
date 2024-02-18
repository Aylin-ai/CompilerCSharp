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

        public virtual TextSpan Span{
            get{
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        public IEnumerable<SyntaxNode> GetChildren(){
            /*
            BindingFlags.Public | BindingFlags.Instance позволяют сказать методу,
            чтобы искал Public свойства в классах
            Порядок расположения свойств в классах важен
            */
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

        public void WriteTo(TextWriter writer){
            PrettyPrint(writer, this);
        }

        /*
        Функция, выводящая синтаксическое дерево. Выводит данные
        о токенах. В зависимости от того, последний это элемент в дереве
        или нет выводит 3 символа, указанных вначале функции
        */
        public static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true){
            //├──
            //│
            //└──

            string marker = isLast ? "└──" : "├──";


            writer.Write(indent);
            writer.Write(marker);
            writer.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null){
                writer.Write($" {t.Value}");
            }
            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            SyntaxNode lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren()){
                PrettyPrint(writer, child, indent, child == lastChild);
            }
        }

        public override string ToString()
        {
            using (var writer = new StringWriter()){
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}