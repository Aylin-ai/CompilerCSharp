using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundScopes;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Lowering;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

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
        private BoundGlobalScope _globalScope;
        public Compilation(params SyntaxTree[] syntaxTrees) : this(null, syntaxTrees) { }

        private Compilation(Compilation previous, params SyntaxTree[] syntaxTrees)
        {
            Previous = previous;
            SyntaxTrees = syntaxTrees.ToList();
        }

        public Compilation Previous { get; }
        public List<SyntaxTree> SyntaxTrees { get; }
        public List<FunctionSymbol> Functions => GlobalScope.Functions;
        public List<VariableSymbol> Variables => GlobalScope.Variables;


        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    BoundGlobalScope? globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTrees);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public IEnumerable<Symbol> GetSymbols()
        {
            Compilation? submission = this;
            HashSet<string>? seenSymbolNames = new HashSet<string>();

            while (submission != null)
            {
                foreach (FunctionSymbol? function in submission.Functions)
                    if (seenSymbolNames.Add(function.Name))
                        yield return function;

                foreach (VariableSymbol? variable in submission.Variables)
                    if (seenSymbolNames.Add(variable.Name))
                        yield return variable;

                submission = submission.Previous;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this, syntaxTree);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            IEnumerable<Diagnostic>? parseDiagnositcs = SyntaxTrees.SelectMany(st => st.Diagnostics);
            IEnumerable<Diagnostic>? diagnostics = parseDiagnositcs.Concat(GlobalScope.Diagnostics);

            if (diagnostics.Any())
            {
                return new EvaluationResult(new DiagnosticBag(diagnostics.ToList()) , null);
            }

            BoundProgram? program = Binder.BindProgram(GlobalScope);

            string? appPath = Environment.GetCommandLineArgs()[0];
            string? appDirectory = Path.GetDirectoryName(appPath);
            string? cfgPath = Path.Combine(appDirectory, "cfg.dot");
            BoundBlockStatement? cfgStatement = !program.Statement.Statements.Any() && program.Functions.Any() 
                                ? program.Functions.Last().Value 
                                : program.Statement;
            ControlFlowGraph? cfg = ControlFlowGraph.Create(cfgStatement);
            using (StreamWriter? streamWriter = new StreamWriter(cfgPath)){
                cfg.WriteTo(streamWriter);
            }

            if (program.Diagnostics.Any())
                return new EvaluationResult(program.Diagnostics, null);

            Evaluator evaluator = new Evaluator(program, variables);
            object value = evaluator.Evaluate();
            return new EvaluationResult([], value);
        }

        public void EmitTree(TextWriter writer)
        {
            BoundProgram? program = Binder.BindProgram(GlobalScope);

            if (program.Statement.Statements.Any())
                program.Statement.WriteTo(writer);
            else
            {
                foreach (KeyValuePair<FunctionSymbol, BoundBlockStatement> functionBody in program.Functions)
                {
                    if (!GlobalScope.Functions.Contains(functionBody.Key))
                        continue;

                    functionBody.Key.WriteTo(writer);
                    writer.WriteLine();
                    functionBody.Value.WriteTo(writer);
                }
            }
        }

        public void EmitTree(FunctionSymbol symbol, TextWriter writer)
        {
            BoundProgram? program = Binder.BindProgram(GlobalScope);
            if (!program.Functions.TryGetValue(symbol, out BoundBlockStatement? body))
                return;

            symbol.WriteTo(writer);
            writer.WriteLine();
            body.WriteTo(writer);
        }
    }
}