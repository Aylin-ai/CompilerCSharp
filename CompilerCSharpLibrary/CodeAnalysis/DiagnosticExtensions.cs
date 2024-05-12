using System.Collections.Generic;
using System.Linq;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    public static class DiagnosticExtensions
    {
        public static bool HasErrors(this List<Diagnostic> diagnostics)
        {
            return diagnostics.Any(d => d.IsError);
        }

        public static bool HasErrors(this IEnumerable<Diagnostic> diagnostics)
        {
            return diagnostics.Any(d => d.IsError);
        }
    }
}