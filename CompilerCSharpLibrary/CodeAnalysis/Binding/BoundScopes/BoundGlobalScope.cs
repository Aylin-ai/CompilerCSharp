using System.Collections.Generic;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.BoundScopes
{
    public sealed class BoundGlobalScope
    {
        public BoundGlobalScope(BoundGlobalScope previous,
                                DiagnosticBag diagnostics,
                                FunctionSymbol mainFunction,
                                FunctionSymbol scriptFunction,
                                List<FunctionSymbol> functions,
                                List<VariableSymbol> variables,
                                List<BoundStatement> statements)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            Functions = functions;
            Variables = variables;
            Statements = statements;
        }

        public BoundGlobalScope Previous { get; }
        public DiagnosticBag Diagnostics { get; }
        public FunctionSymbol MainFunction { get; }
        public FunctionSymbol ScriptFunction { get; }
        public List<FunctionSymbol> Functions { get; }
        public List<VariableSymbol> Variables { get; }
        public List<BoundStatement> Statements { get; }
    }
}