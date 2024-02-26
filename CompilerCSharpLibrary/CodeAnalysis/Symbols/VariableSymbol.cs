namespace CompilerCSharpLibrary.CodeAnalysis.Symbols
{
    /*
    Класс, представляющий переменную. Содержит имя и тип переменной
    */
    public sealed class VariableSymbol : Symbol
    {
        internal VariableSymbol(string name, bool isReadOnly, Type type) : base(name)
        {
            IsReadOnly = isReadOnly;
            Type = type;
        }

        public bool IsReadOnly { get; }
        public Type Type { get; }

        public override SymbolKind Kind => SymbolKind.Variable;

        public override string ToString() => Name;
    }
}