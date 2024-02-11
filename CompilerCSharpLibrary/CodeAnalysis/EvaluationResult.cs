namespace CompilerCSharpLibrary.CodeAnalysis
{
    public sealed class EvaluationResult{
        public EvaluationResult(DiagnosticBag diagnostics, object value){
            Diagnostics = diagnostics;
            Value = value;
        }

        public DiagnosticBag Diagnostics { get; }
        public object Value { get; }
    }
}