using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;

bool showTree = false;
var variables = new Dictionary<VariableSymbol, object>();

while (true){
    Console.Write("> ");
    string line = Console.ReadLine();
    if (string.IsNullOrEmpty(line))
        return;

    //Отладочные команды для показа дерева и очистки консоли
    if (line == "#showTree"){
        showTree = !showTree;
        Console.WriteLine(showTree ? "Showing parse tree" : "Not showing parse tree");
        continue;
    } else if (line == "#clear"){
        Console.Clear();
        continue;
    }

    SyntaxTree syntaxTree = SyntaxTree.Parse(line);
    Compilation compilation = new Compilation(syntaxTree);
    EvaluationResult result = compilation.Evaluate(variables);

    DiagnosticBag diagnostics = result.Diagnostics;

    if (showTree)
        syntaxTree.Root.WriteTo(Console.Out);

    //Выдает ошибку с некоторыми выражениями (например, 2++)
    if (diagnostics.Any()){
        foreach(var diagnostic in diagnostics){
            Console.WriteLine(diagnostic);

            string prefix = line.Substring(0, diagnostic.Span.Start);
            string error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
            string suffix = line.Substring(diagnostic.Span.End);

            Console.Write("    ");
            Console.Write(prefix);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(error);
            Console.ResetColor();
            Console.Write(suffix);
            Console.WriteLine();
        }
        Console.WriteLine();
    } else{
        Console.WriteLine(result.Value);
    }
}

