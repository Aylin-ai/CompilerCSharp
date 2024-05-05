using System.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, содержащий список с пойманными ошибками, а также
    методы для их отлавливания и добавления в список
    */
    public sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        public DiagnosticBag() {}
        public DiagnosticBag(List<Diagnostic> diagnostics) {
            _diagnostics = diagnostics;
        }
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }

        private void Report(TextLocation location, string message)
        {
            Diagnostic diagnostic = new Diagnostic(location, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextLocation location, string text, TypeSymbol type)
        {
            string message = $"The number '{text}' isn't valid '{type}'.";
            Report(location, message);
        }

        public void ReportBadCharacter(TextLocation location, char current)
        {
            string message = $"Bad character input: '{current}'.";
            Report(location, message);
        }

        public void ReportUnexpectedToken(TextLocation location, SyntaxKind actualKind, SyntaxKind expectedKind)
        {
            string message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
            Report(location, message);
        }

        public void ReportUndefinedUnaryOperator(TextLocation location, string operatorText, TypeSymbol operandType)
        {
            string message = $"Unary operator '{operatorText}' is not defined for type '{operandType}'";
            Report(location, message);
        }

        internal void ReportUndefinedBinaryOperator(TextLocation location, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            string message = $"Binary operator '{operatorText}' is not defined for type '{leftType}' and '{rightType}'";
            Report(location, message);
        }

        public void ReportUndefinedVariable(TextLocation location, string name)
        {
            string message = $"Variable '{name}' doesn't exist.";
            Report(location, message);
        }

        public void ReportNotAVariable(TextLocation location, string name)
        {
            string message = $"'{name}' is not a variable.";
            Report(location, message);
        }

        public void ReportSymbolAlreadyDeclared(TextLocation location, string name)
        {
            string message = $"'{name}' is already declared.";
            Report(location, message);
        }

        public void ReportCannotConvert(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            string message = $"Cannot conver type '{fromType}' to '{toType}'.";
            Report(location, message);
        }

        public void ReportCannotAssign(TextLocation location, string name)
        {
            string message = $"Variable '{name}' is read-only and cannot be assigned to.";
            Report(location, message);
        }

        public void ReportUnterminatedString(TextLocation location)
        {
            string message = $"Unterminated string literal.";
            Report(location, message);
        }

        public void ReportUndefinedFunction(TextLocation location, string name)
        {
            string message = $"Function '{name}' doesn't exist.";
            Report(location, message);
        }

        public void ReportNotAFunction(TextLocation location, string name)
        {
            string message = $"'{name}' is not a function.";
            Report(location, message);
        }

        public void ReportWrongArgumentCount(TextLocation location, string name, int expectedCount, int actualCount)
        {
            string message = $"Function '{name}' requires {expectedCount} but was given {actualCount}.";
            Report(location, message);
        }

        public void ReportWrongArgumentType(TextLocation location, string name,
        TypeSymbol expectedType, TypeSymbol actualType)
        {
            string message = $"Parameter '{name}' requires a value of type '{expectedType}' but was given a value of type '{actualType}'.";
            Report(location, message);
        }

        public void ReportExpressionMustHaveValue(TextLocation location)
        {
            string message = $"Expression must have a value.";
            Report(location, message);
        }

        public void ReportUndefinedType(TextLocation location, string name)
        {
            string message = $"Type '{name}' doesn't exist.";
            Report(location, message);
        }

        public void ReportCannotConvertImplicitly(TextLocation location, TypeSymbol fromType, TypeSymbol toType)
        {
            string message = $"Cannot conver type '{fromType}' to '{toType}'. An explicit conversion exists; are you missing a cast?";
            Report(location, message);
        }

        public void ReportParameterAlreadyDeclared(TextLocation location, string parameterName)
        {
            string message = $"A parameter with the name '{parameterName}' is already declared.";
            Report(location, message);
        }

        public void ReportInvalidBreakOrContinue(TextLocation location, string text)
        {
            string message = $"The keyword '{text}' is can only be used inside loops.";
            Report(location, message);
        }

        public void ReportInvalidReturn(TextLocation location)
        {
            string message = $"The 'return' keyword can only be used inside of function.";
            Report(location, message);
        }

        public void ReportInvalidReturnExpression(TextLocation location, string functionName)
        {
            string message = $"Sinsce the function '{functionName}' does not return" +
            $" a value the 'return' keyword cannot be followed by an expression.";
            Report(location, message);
        }

        public void ReportAllPathMustReturn(TextLocation location)
        {
            string message = $"Not all code paths return a value.";
            Report(location, message);
        }

        public void ReportMissingReturnExpression(TextLocation location, TypeSymbol returnType)
        {
            string message = $"An expression of type '{returnType}' is expected.";
            Report(location, message);
        }

        internal void ReportInvalidExpressionStatement(TextLocation location)
        {
            string message = $"Only assignment and call expressions can be used as a statement.";
            Report(location, message);
        }

        public void XXX_ReportFunctionsAreUnsupported(TextLocation location)
        {
            string message = $"Functions with return values are unsupported.";
            Report(location, message);
        }

        
    }
}