namespace CompilerCSharpLibrary.CodeAnalysis.Symbols
{
    public sealed class ParameterSymbol : LocalVariableSymbol{
        public ParameterSymbol(string name, TypeSymbol type, int ordinal) : base(name, isReadOnly: true, type, null)
        {
            Ordinal = ordinal;
        }

        public override SymbolKind Kind => SymbolKind.Parameter;

        //Индекс параметра
        public int Ordinal { get; }
    }
}