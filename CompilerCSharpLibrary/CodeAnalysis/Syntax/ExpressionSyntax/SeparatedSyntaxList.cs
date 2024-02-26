using System.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{
    //Это не SyntaxNode, т.е. сам по себе не узел в синтаксическом дереве
    public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T>
        where T : SyntaxNode
    {
        private readonly List<SyntaxNode> _nodesAndSeparators;

        public SeparatedSyntaxList(List<SyntaxNode> nodesAndSeparators)
        {
            _nodesAndSeparators = nodesAndSeparators;
        }

        public int Count => (_nodesAndSeparators.Count + 1) / 2;

        public T this[int index] => (T)_nodesAndSeparators[index * 2];

        public SyntaxToken GetSeparator(int index) {
            if (index == Count - 1)
                return null;
                
            return (SyntaxToken) _nodesAndSeparators[index * 2 + 1];
        } 


        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++){
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override List<SyntaxNode> GetWithSeparators() => _nodesAndSeparators;
    }

    public abstract class SeparatedSyntaxList{
        public abstract List<SyntaxNode> GetWithSeparators();
    }
}