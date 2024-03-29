namespace CompilerCSharpLibrary.CodeAnalysis.Symbols
{
    /*
    Класс, представляющий переменную. Содержит имя и тип переменной
    */
    public abstract class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, TypeSymbol type) : base(name)
        {
            IsReadOnly = isReadOnly;
            Type = type;
        }

        public bool IsReadOnly { get; }
        public TypeSymbol Type { get; }
    }
}