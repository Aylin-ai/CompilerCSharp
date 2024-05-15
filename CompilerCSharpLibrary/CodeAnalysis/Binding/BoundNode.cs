using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    /*
    Абстрактный класс, отражающий узел в АСД. Содержит
    в себе вид узла.
    */
    public abstract class BoundNode
    {

        #region Поля класса

        public abstract BoundNodeKind Kind { get; }

        #endregion

        #region Методы класса

        public IEnumerable<BoundNode> GetChildren()
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
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
                {
                    BoundNode? child = (BoundNode)property.GetValue(this);
                    if (child != null)
                        yield return child;
                }
                //Можно ли преобразовать property.PropertyType в IEnumerable<SyntaxNode>?
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    IEnumerable<BoundNode>? children = (IEnumerable<BoundNode>)property.GetValue(this);
                    foreach (BoundNode? child in children)
                    {
                        if (child != null)
                            yield return child;
                    }
                }
            }
        }

        // private IEnumerable<(string Name, object Value)> GetProperties(){
        //     /*
        //     BindingFlags.Public | BindingFlags.Instance позволяют сказать методу,
        //     чтобы искал Public свойства в классах
        //     Порядок расположения свойств в классах важен
        //     */
        //     PropertyInfo[]? properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //     foreach (PropertyInfo? property in properties){
        //         if (property.Name == nameof(Kind) ||
        //         property.Name == nameof(BoundBinaryExpression.Op))
        //             continue;
        //         //Можно ли преобразовать property.PropertyType в SyntaxNode?
        //         if (typeof(BoundNode).IsAssignableFrom(property.PropertyType) ||
        //         typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType)){
        //             continue;
        //         }
        //         object? value = property.GetValue(this);
        //         if (value != null)
        //             yield return (property.Name, value);
        //     }
        // }

        public override string ToString()
        {
            using (StringWriter? writer = new StringWriter())
            {
                this.WriteTo(writer);
                return writer.ToString();
            }
        }

        #endregion

    }
}