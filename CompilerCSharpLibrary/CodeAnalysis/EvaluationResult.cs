using System.Collections.Generic;
using System.Linq;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, содержащий результат вычисления выражения
    и все пойманные ошибки
    */
    public sealed class EvaluationResult{
        public EvaluationResult(DiagnosticBag diagnostics, object value){
            Diagnostics = diagnostics;
            Value = value;
            ErrorDiagnostics = Diagnostics.Where(d => d.IsError).ToList();
            WarningDiagnostics = Diagnostics.Where(d => d.IsWarning).ToList();
        }

        public DiagnosticBag Diagnostics { get; }
        public List<Diagnostic> ErrorDiagnostics { get; }
        public List<Diagnostic> WarningDiagnostics { get; }
        public object Value { get; }
    }
}