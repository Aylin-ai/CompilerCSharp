using System.Collections;
using System.Collections.Generic;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax
{

    //Это не SyntaxNode, т.е. сам по себе не узел в синтаксическом дереве
    public sealed class SeparatedSyntaxList<T> : SeparatedSyntaxList, IEnumerable<T>
        where T : SyntaxNode
    {

        #region Поля класса

        private readonly List<SyntaxNode> _nodesAndSeparators;
        public int Count => (_nodesAndSeparators.Count + 1) / 2;
        public T this[int index] => (T)_nodesAndSeparators[index * 2];

        #endregion

        #region Конструкторы класса

        public SeparatedSyntaxList(List<SyntaxNode> nodesAndSeparators)
        {
            _nodesAndSeparators = nodesAndSeparators;
        }

        #endregion

        #region Методы класса

        public SyntaxToken GetSeparator(int index)
        {
            if (index == Count - 1)
                return null;

            return (SyntaxToken)_nodesAndSeparators[index * 2 + 1];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override List<SyntaxNode> GetWithSeparators() => _nodesAndSeparators;

        #endregion
    }

    #region Внутренние классы

    #region Абстрактный класс SeparatedSyntaxList

    public abstract class SeparatedSyntaxList
    {
        public abstract List<SyntaxNode> GetWithSeparators();
    }

    #endregion

    #endregion

}