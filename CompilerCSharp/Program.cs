using CompilerCSharpLibrary.CodeAnalysis;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.IO;
using Mono.Options;

namespace CompilerCSharp
{
    internal static class Program
    {
        static int Main(string[] args)
        {
            var outputPath = (string)null;
            var moduleName = (string)null;
            var referencePaths = new List<string>();
            var sourcePaths = new List<string>();
            var helpRequested = false;

            var options = new OptionSet{
                "usage: CompilerCSharp <source-paths> [options]",
                { "r=", "The {path} of an assembly to reference", v => referencePaths.Add(v) },
                { "o=", "The output {path} of an assembly to create", v => outputPath = v },
                { "m=", "The {name} of the module", v => moduleName = v },
                { "<>", v => sourcePaths.Add(v) },
                { "?|h|help", "Prints help", v => helpRequested = true },
            };

            options.Parse(args);

            if (helpRequested)
            {
                options.WriteOptionDescriptions(Console.Out);
                return 0;
            }

            if (sourcePaths.Count == 0)
            {
                Console.Error.WriteLine("error: need at least one source file");
                return 1;
            }

            if (outputPath == null)
            {
                outputPath = Path.ChangeExtension(sourcePaths[0], ".exe");
            }

            if (moduleName == null)
                moduleName = Path.GetFileNameWithoutExtension(outputPath);

            List<SyntaxTree>? syntaxTrees = new List<SyntaxTree>();
            bool hasErrors = false;

            foreach (string? path in sourcePaths)
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

            foreach (string? path in referencePaths)
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
            var diagnostics = compilation.Emit(moduleName, referencePaths.ToArray(), outputPath);
            
            if (diagnostics.Any())
            {
                Console.Error.WriteDiagnostics(diagnostics);
                return 1;
            }

            return 0;
        }
    }
}
