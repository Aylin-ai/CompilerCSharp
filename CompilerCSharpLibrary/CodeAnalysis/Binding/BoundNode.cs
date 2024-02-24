using System.Reflection;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    /*
    Абстрактный класс, отражающий узел в АСД. Содержит
    в себе вид узла.
    */
    public abstract class BoundNode{
        public abstract BoundNodeKind Kind { get; }

        public IEnumerable<BoundNode> GetChildren(){
            /*
            BindingFlags.Public | BindingFlags.Instance позволяют сказать методу,
            чтобы искал Public свойства в классах
            Порядок расположения свойств в классах важен
            */
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties){
                //Можно ли преобразовать property.PropertyType в SyntaxNode?
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType)){
                    var child = (BoundNode)property.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                //Можно ли преобразовать property.PropertyType в IEnumerable<SyntaxNode>?
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType)){
                    var children = (IEnumerable<BoundNode>)property.GetValue(this);
                    foreach (var child in children){
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }

        private IEnumerable<(string Name, object Value)> GetProperties(){
            /*
            BindingFlags.Public | BindingFlags.Instance позволяют сказать методу,
            чтобы искал Public свойства в классах
            Порядок расположения свойств в классах важен
            */
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties){
                if (property.Name == nameof(Kind) ||
                property.Name == nameof(BoundBinaryExpression.Op))
                    continue;
                //Можно ли преобразовать property.PropertyType в SyntaxNode?
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType) ||
                typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType)){
                    continue;
                }
                var value = property.GetValue(this);
                if (value != null)
                    yield return (property.Name, value);
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
        public static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true){
            //├──
            //│
            //└──

            string marker = isLast ? "└──" : "├──";


            writer.Write(indent);
            writer.Write(marker);

            var text = GetText(node);
            writer.Write(text);

            var isFirstProperty = true;

            foreach (var p in node.GetProperties()){
                if (isFirstProperty)
                    isFirstProperty = false;
                else
                    writer.Write(",");

                writer.Write($" {p.Name} = {p.Value}");
            }

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            BoundNode lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren()){
                PrettyPrint(writer, child, indent, child == lastChild);
            }
        }

        private static string GetText(BoundNode node)
        {
            if (node is BoundBinaryExpression b)
                return b.Op.Kind.ToString() + "Expression";
            
            if (node is BoundUnaryExpression u)
                return u.Op.Kind.ToString() + "Expression";

            return node.Kind.ToString();
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