using System.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, содержащий список с пойманными ошибками, а также
    методы для их отлавливания и добавления в список
    */
    public sealed class DiagnosticBag : IEnumerable<Diagnostic>{
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }

        private void Report(TextSpan span, string message){
            Diagnostic diagnostic = new Diagnostic(span, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextSpan span, string text, Type type)
        {
            string message = $"The number {text} isn't valid {type}.";
            Report(span, message);
        }

        public void ReportBadCharacter(int position, char current)
        {
            TextSpan textSpan = new TextSpan(position, 1);
            string message = $"Bad character input: '{current}'.";
            Report(textSpan, message);
        }

        public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            string message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
            Report(span, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type operandType)
        {
            string message = $"Unary operator '{operatorText}' is not defined for type {operandType}";
            Report(span, message);
        }

        internal void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, Type leftType, Type rightType)
        {
            string message = $"Binary operator '{operatorText}' is not defined for type {leftType} and {rightType}";
            Report(span, message);
        }

        public void ReportUndefinedName(TextSpan span, string name)
        {
            string message = $"Variable '{name}' doesn't exist.";
            Report(span, message);
        }
    }
}