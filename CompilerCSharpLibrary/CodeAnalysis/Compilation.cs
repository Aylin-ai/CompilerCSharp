using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundScopes;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

using ReflectionBindingFlags = System.Reflection.BindingFlags;
using CompilerCSharpLibrary.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, представляющий собой компиляцию. Создан для более
    удобного взаимодействия с остальными классами.
    Содержит в себе синтаксическое дерево выражения и
    метод для вычисления выражения
    */
    public sealed class Compilation
    {

        #region Поля класса

        private BoundGlobalScope _globalScope;
        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    BoundGlobalScope? globalScope = Binder.BindGlobalScope(IsScript, Previous?.GlobalScope, SyntaxTrees);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public bool IsScript { get; }
        public Compilation Previous { get; }
        public List<SyntaxTree> SyntaxTrees { get; }
        public FunctionSymbol MainFunction => GlobalScope.MainFunction;
        public List<FunctionSymbol> Functions => GlobalScope.Functions;
        public List<VariableSymbol> Variables => GlobalScope.Variables;

        #endregion

        #region Конструкторы класса

        private Compilation(bool isScript, Compilation previous, params SyntaxTree[] syntaxTrees)
        {
            IsScript = isScript;
            Previous = previous;
            SyntaxTrees = syntaxTrees.ToList();
        }

        #endregion

        #region Методы класса

        public static Compilation Create(params SyntaxTree[] syntaxTrees)
        {
            return new Compilation(isScript: false, previous: null, syntaxTrees);
        }

        public static Compilation CreateScript(Compilation previous, params SyntaxTree[] syntaxTrees)
        {
            return new Compilation(isScript: true, previous, syntaxTrees);
        }

        public IEnumerable<Symbol> GetSymbols()
        {
            Compilation? submission = this;
            HashSet<string>? seenSymbolNames = new HashSet<string>();
            var builtinFunctions = BuiltInFunctions.GetAll().ToList();

            while (submission != null)
            {
                foreach (FunctionSymbol? function in submission.Functions)
                    if (seenSymbolNames.Add(function.Name))
                        yield return function;

                foreach (VariableSymbol? variable in submission.Variables)
                    if (seenSymbolNames.Add(variable.Name))
                        yield return variable;

                foreach (var builtin in builtinFunctions)
                    if (seenSymbolNames.Add(builtin.Name))
                        yield return builtin;

                submission = submission.Previous;
            }
        }

        private BoundProgram GetProgram()
        {
            var previous = Previous == null ? null : Previous.GetProgram();
            return Binder.BindProgram(IsScript, previous, GlobalScope);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            if (GlobalScope.Diagnostics.Any())
                return new EvaluationResult(GlobalScope.Diagnostics, null);

            BoundProgram? program = GetProgram();

            // string? appPath = Environment.GetCommandLineArgs()[0];
            // string? appDirectory = Path.GetDirectoryName(appPath);
            // string? cfgPath = Path.Combine(appDirectory, "cfg.dot");
            // BoundBlockStatement? cfgStatement = program.Functions.Last().Value;
            // ControlFlowGraph? cfg = ControlFlowGraph.Create(cfgStatement);
            // using (StreamWriter? streamWriter = new StreamWriter(cfgPath))
            // {
            //     cfg.WriteTo(streamWriter);
            // }

            if (program.ErrorDiagnostics.Any())
                return new EvaluationResult(program.Diagnostics, null);

            Evaluator evaluator = new Evaluator(program, variables);
            object value = evaluator.Evaluate();

            DiagnosticBag diagnostics = new DiagnosticBag();
            diagnostics.AddRange(program.WarningDiagnostics);

            return new EvaluationResult(diagnostics, value);
        }

        public void EmitTree(TextWriter writer)
        {
            if (GlobalScope.MainFunction != null)
                EmitTree(GlobalScope.MainFunction, writer);
            else if (GlobalScope.ScriptFunction != null)
                EmitTree(GlobalScope.ScriptFunction, writer);
        }

        /*
        Выводит имя и параметры функции. Если у функции нет тела, то return. 
        В ином случае выводит тело функции
        */
        public void EmitTree(FunctionSymbol symbol, TextWriter writer)
        {
            BoundProgram? program = GetProgram();

            symbol.WriteTo(writer);
            writer.WriteLine();
            if (!program.Functions.TryGetValue(symbol, out BoundBlockStatement? body))
                return;
            body.WriteTo(writer);
        }

        public DiagnosticBag Emit(string moduleName, string[] references, string outputPath)
        {
            var parseDiagnostics = SyntaxTrees.SelectMany(st => st.Diagnostics);

            var diagnostics = parseDiagnostics.Concat(GlobalScope.Diagnostics);
            var errorDiagnostics = diagnostics.Where(d => d.IsError);
            if (errorDiagnostics.Any())
            {
                var diagnostic = new DiagnosticBag();
                diagnostic.AddRange(diagnostics);
                return diagnostic;
            }

            var program = GetProgram();

            if (program.Diagnostics.Any())
                return program.Diagnostics;

            return Emitter.Emit(program, moduleName, references, outputPath);
        }

        #endregion

    }
}