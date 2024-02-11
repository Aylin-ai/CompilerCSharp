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

            Syntax.Diagnostics.AddRange(binder.Diagnostics);
            if (Syntax.Diagnostics.Any()){
                return new EvaluationResult(Syntax.Diagnostics, null);
            }

            Evaluator evaluator = new Evaluator(boundExpression);
            object value = evaluator.Evaluate();
            return new EvaluationResult([], value);
        }
    }
}