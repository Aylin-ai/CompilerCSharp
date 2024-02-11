using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;

bool showTree = false;

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
    EvaluationResult result = compilation.Evaluate();

    DiagnosticBag diagnostics = result.Diagnostics;

    if (showTree)
        PrettyPrint(syntaxTree.Root);

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

/*
Функция, выводящая синтаксическое дерево. Выводит данные
о токенах. В зависимости от того, последний это элемент в дереве
или нет выводит 3 символа, указанных вначале функции
*/
static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true){
    //├──
    //│
    //└──

    string marker = isLast ? "└──" : "├──";


    Console.Write(indent);
    Console.Write(marker);
    Console.Write(node.Kind);

    if (node is SyntaxToken t && t.Value != null){
        Console.Write($" {t.Value}");
    }
    Console.WriteLine();

    indent += isLast ? "   " : "│  ";

    SyntaxNode lastChild = node.GetChildren().LastOrDefault();

    foreach (var child in node.GetChildren()){
        PrettyPrint(child, indent, child == lastChild);
    }
}

