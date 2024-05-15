using System.Collections.Generic;
using System.Linq;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.BoundScopes
{
    public sealed class BoundScope
    {

        #region Поля класса

        private Dictionary<string, VariableSymbol> _variables;
        private Dictionary<string, FunctionSymbol> _functions;
        private Dictionary<string, Symbol> _symbols;
        public BoundScope Parent { get; }

        #endregion

        #region Конструкторы класса

        public BoundScope(BoundScope parent)
        {
            Parent = parent;
        }

        #endregion

        #region Методы класса

        public bool TryDeclareVariable(VariableSymbol variable) => TryDeclareSymbol(variable);

        public bool TryDeclareFunction(FunctionSymbol function) => TryDeclareSymbol(function);

        public List<VariableSymbol> GetDeclaredVariables() => GetDeclaredSymbols<VariableSymbol>();

        public List<FunctionSymbol> GetDeclaredFunctions() => GetDeclaredSymbols<FunctionSymbol>();

        public bool TryDeclareSymbol<TSymbol>(TSymbol symbol)
            where TSymbol : Symbol
        {
            if (_symbols == null)
                _symbols = new Dictionary<string, Symbol>();
            else if (_symbols.ContainsKey(symbol.Name))
                return false;

            _symbols.Add(symbol.Name, symbol);
            return true;
        }

        public Symbol TryLookupSymbol(string name)
        {
            if (_symbols != null && _symbols.TryGetValue(name, out Symbol? symbol))
                return symbol;

            return Parent?.TryLookupSymbol(name);
        }

        private List<TSymbol> GetDeclaredSymbols<TSymbol>()
            where TSymbol : Symbol
        {
            if (_symbols == null)
                return new List<TSymbol>();

            return _symbols.Values.OfType<TSymbol>().ToList();
        }

        #endregion

    }
}