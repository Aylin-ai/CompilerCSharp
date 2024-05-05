using CompilerCSharpLibrary.CodeAnalysis;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.IO;

namespace CompilerCSharp
{
    internal static class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("usage: CompilerCSharp <source-paths>");
                return 1;
            }

            IEnumerable<string>? paths = GetFilePaths(args);
            List<SyntaxTree>? syntaxTrees = new List<SyntaxTree>();
            bool hasErrors = false;

            foreach (string? path in paths)
            {
                if (!File.Exists(path))
                {
                    Console.Error.WriteLine("error: file '{path}' doesn't exist");
                    hasErrors = true;
                    continue;
                }
                SyntaxTree? syntaxTree = SyntaxTree.Load(path);
                syntaxTrees.Add(syntaxTree);
            }

            if (hasErrors)
                return 1;

            Compilation? compilation = Compilation.Create(syntaxTrees.ToArray());
            EvaluationResult? result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            if (!result.Diagnostics.Any())
            {
                if (result.Value != null)
                    Console.Out.WriteLine(result.Value);
            }
            else
            {
                Console.Error.WriteDiagnostics(result.Diagnostics);
                return 1;
            }

            return 0;
        }

        private static IEnumerable<string> GetFilePaths(IEnumerable<string> args)
        {
            SortedSet<string>? result = new SortedSet<string>();
            foreach (string? path in args)
            {
                if (Directory.Exists(path))
                {
                    result.UnionWith(Directory.EnumerateFiles(path, "*.ms", SearchOption.AllDirectories));
                }
                else
                {
                    result.Add(path);
                }
            }

            return result;
        }
    }
}
