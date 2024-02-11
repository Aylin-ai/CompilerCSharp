using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.CodeAnalysis.Binding;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    public sealed class Compilation{
        public Compilation(SyntaxTree syntax){
            Syntax = syntax;
        }

        public SyntaxTree Syntax { get; }

        public EvaluationResult Evaluate(){
            Binder binder = new Binder();
            BoundExpression boundExpression = binder.BindExpression(Syntax.Root);

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToArray();
            if (diagnostics.Any()){
                return new EvaluationResult(diagnostics, null);
            }

            Evaluator evaluator = new Evaluator(boundExpression);
            object value = evaluator.Evaluate();
            return new EvaluationResult(Array.Empty<string>(), value);
        }
    }
}