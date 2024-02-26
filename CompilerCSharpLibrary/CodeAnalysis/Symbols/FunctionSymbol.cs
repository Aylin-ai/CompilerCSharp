namespace CompilerCSharpLibrary.CodeAnalysis.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, List<ParameterSymbol> parameters, TypeSymbol type) : base(name) {
            Parameters = parameters;
            Type = type;
        }
        public override SymbolKind Kind => SymbolKind.Function;

        public List<ParameterSymbol> Parameters { get; }
        public TypeSymbol Type { get; }
    }
}