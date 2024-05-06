using System.Collections.Generic;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;

namespace CompilerCSharpLibrary.CodeAnalysis.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, List<ParameterSymbol> parameters, TypeSymbol type,
        FunctionDeclarationSyntax declaration = null) : base(name) {
            Parameters = parameters;
            Type = type;
            Declaration = declaration;
        }
        public override SymbolKind Kind => SymbolKind.Function;

        public List<ParameterSymbol> Parameters { get; }
        public TypeSymbol Type { get; }
        public FunctionDeclarationSyntax Declaration { get; }
    }
}