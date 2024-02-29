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

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTrees);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this, syntaxTree);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            var parseDiagnositcs = SyntaxTrees.SelectMany(st => st.Diagnostics);
            var diagnostics = parseDiagnositcs.Concat(GlobalScope.Diagnostics);

            if (diagnostics.Any())
            {
                return new EvaluationResult(new DiagnosticBag(diagnostics.ToList()) , null);
            }

            var program = Binder.BindProgram(GlobalScope);

            var appPath = Environment.GetCommandLineArgs()[0];
            var appDirectory = Path.GetDirectoryName(appPath);
            var cfgPath = Path.Combine(appDirectory, "cfg.dot");
            var cfgStatement = !program.Statement.Statements.Any() && program.Functions.Any() 
                                ? program.Functions.Last().Value 
                                : program.Statement;
            var cfg = ControlFlowGraph.Create(cfgStatement);
            using (var streamWriter = new StreamWriter(cfgPath)){
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
            var program = Binder.BindProgram(GlobalScope);

            if (program.Statement.Statements.Any())
                program.Statement.WriteTo(writer);
            else
            {
                foreach (var functionBody in program.Functions)
                {
                    if (!GlobalScope.Functions.Contains(functionBody.Key))
                        continue;

                    functionBody.Key.WriteTo(writer);
                    functionBody.Value.WriteTo(writer);
                }
            }
        }
    }
}