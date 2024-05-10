using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions;

namespace CompilerCSharpLibrary.CodeAnalysis.Symbols
{
    public class LocalVariableSymbol : VariableSymbol
    {
        public LocalVariableSymbol(string name, bool isReadOnly, TypeSymbol type, BoundConstant constant) : base(name, isReadOnly, type, constant)
        {
        }

        public override SymbolKind Kind => SymbolKind.LocalVariable;
    }
}