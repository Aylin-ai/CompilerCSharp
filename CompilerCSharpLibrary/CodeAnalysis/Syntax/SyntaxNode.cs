using System.Reflection;
using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Абстрактный класс, представляющий узел в синтаксическом дереве.
    От него исходят все остальные классы, включая SyntaxToken.
    Содержит тип токена и метод получения дочерних токенов.
    */
    public abstract class SyntaxNode
    {
        protected SyntaxNode(SyntaxTree syntaxTree){
            SyntaxTree = syntaxTree;
        }
        
        public SyntaxTree SyntaxTree { get; }
        public abstract SyntaxKind Kind { get; }

        public virtual TextSpan Span
        {
            get
            {
                TextSpan? first = GetChildren().First().Span;
                TextSpan? last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        public TextLocation Location => new TextLocation(SyntaxTree.Text, Span);

        public IEnumerable<SyntaxNode> GetChildren()
        {
            /*
            BindingFlags.Public | BindingFlags.Instance позволяют сказать методу,
            чтобы искал Public свойства в классах
            Порядок расположения свойств в классах важен
            */
            PropertyInfo[]? properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo? property in properties)
            {
                //Можно ли преобразовать property.PropertyType в SyntaxNode?
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    SyntaxNode? child = (SyntaxNode)property.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                else if (typeof(SeparatedSyntaxList).IsAssignableFrom(property.PropertyType))
                {
                    SeparatedSyntaxList? separatedSyntaxList = (SeparatedSyntaxList)property.GetValue(this);
                    foreach (SyntaxNode? child in separatedSyntaxList.GetWithSeparators())
                    {
                        yield return child;
                    }
                }
                //Можно ли преобразовать property.PropertyType в IEnumerable<SyntaxNode>?
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    IEnumerable<SyntaxNode>? children = (IEnumerable<SyntaxNode>)property.GetValue(this);
                    foreach (SyntaxNode? child in children)
                    {
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }

        public SyntaxToken GetLastToken()
        {
            if (this is SyntaxToken token)
                return token;

            return GetChildren().Last().GetLastToken();
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        /*
        Функция, выводящая синтаксическое дерево. Выводит данные
        о токенах. В зависимости от того, последний это элемент в дереве
        или нет выводит 3 символа, указанных вначале функции
        */
        public static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            //├──
            //│
            //└──

            string marker = isLast ? "└──" : "├──";


            writer.Write(indent);
            writer.Write(marker);
            writer.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                writer.Write($" {t.Value}");
            }
            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            SyntaxNode lastChild = node.GetChildren().LastOrDefault();

            foreach (SyntaxNode? child in node.GetChildren())
            {
                PrettyPrint(writer, child, indent, child == lastChild);
            }
        }

        public override string ToString()
        {
            using (StringWriter? writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }
    }
}