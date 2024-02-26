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
    public sealed class Compilation{
        private BoundGlobalScope _globalScope;
        public Compilation(SyntaxTree syntax) : this(null, syntax) {}

        private Compilation(Compilation previous, SyntaxTree syntax){
            Previous = previous;
            Syntax = syntax;
        }

        public Compilation Previous { get; }
        public SyntaxTree Syntax { get; }

        internal BoundGlobalScope GlobalScope{
            get {
                if (_globalScope == null){
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, Syntax.Root);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree){
            return new Compilation(this, syntaxTree);
        }

        public void EmitTree(TextWriter writer)
        {
            var statement = GetStatement();
            statement.WriteTo(writer);
        }

        private BoundBlockStatement GetStatement()
        {
            var result = GlobalScope.Statement;
            return Lowerer.Lower(result);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables){
            Syntax.Diagnostics.AddRange(GlobalScope.Diagnostics);
            if (Syntax.Diagnostics.Any()){
                return new EvaluationResult(Syntax.Diagnostics, null);
            }

            var statement = GetStatement();

            Evaluator evaluator = new Evaluator(statement, variables);
            object value = evaluator.Evaluate();
            return new EvaluationResult([], value);
        }
    }
}