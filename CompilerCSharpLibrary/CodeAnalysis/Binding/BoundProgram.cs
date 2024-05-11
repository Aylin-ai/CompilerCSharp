using System.Collections.Generic;
using System.Linq;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundScopes;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    public sealed class BoundProgram
    {
        public BoundProgram(BoundProgram previous,
                            DiagnosticBag diagnostics,
                            FunctionSymbol mainFunction,
                            FunctionSymbol scriptFunction,
                            Dictionary<FunctionSymbol, BoundBlockStatement> functions)
        {
            Previous = previous;
            Diagnostics = diagnostics;
            MainFunction = mainFunction;
            ScriptFunction = scriptFunction;
            Functions = functions;
            ErrorDiagnostics = Diagnostics.Where(d => d.IsError).ToList();
            WarningDiagnostics = Diagnostics.Where(d => d.IsWarning).ToList();
        }

        public BoundProgram Previous { get; }
        public DiagnosticBag Diagnostics { get; }
        public List<Diagnostic> ErrorDiagnostics { get; }
        public List<Diagnostic> WarningDiagnostics { get; }
        public FunctionSymbol MainFunction { get; }
        public FunctionSymbol ScriptFunction { get; }
        public Dictionary<FunctionSymbol, BoundBlockStatement> Functions { get; }
    }
}