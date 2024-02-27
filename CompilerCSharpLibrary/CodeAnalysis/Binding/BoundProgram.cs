using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundScopes;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    public sealed class BoundProgram
    {
        public BoundProgram(BoundGlobalScope globalScope, DiagnosticBag diagnostics, 
        Dictionary<FunctionSymbol, BoundBlockStatement> functionBodies)
        {
            GlobalScope = globalScope;
            Diagnostics = diagnostics;
            FunctionBodies = functionBodies;
        }

        public BoundGlobalScope GlobalScope { get; }
        public DiagnosticBag Diagnostics { get; }
        public Dictionary<FunctionSymbol, BoundBlockStatement> FunctionBodies { get; }
    }
}