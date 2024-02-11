using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.CodeAnalysis.Binding;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, представляющий собой компиляцию. Создан для более
    удобного взаимодействия с остальными классами.
    Содержит в себе синтаксическое дерево выражения и
    метод для вычисления выражения
    */
    public sealed class Compilation{
        public Compilation(SyntaxTree syntax){
            Syntax = syntax;
        }

        public SyntaxTree Syntax { get; }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables){
            Binder binder = new Binder(variables);
            BoundExpression boundExpression = binder.BindExpression(Syntax.Root);

            Syntax.Diagnostics.AddRange(binder.Diagnostics);
            if (Syntax.Diagnostics.Any()){
                return new EvaluationResult(Syntax.Diagnostics, null);
            }

            Evaluator evaluator = new Evaluator(boundExpression, variables);
            object value = evaluator.Evaluate();
            return new EvaluationResult([], value);
        }
    }
}