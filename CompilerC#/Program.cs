using CompilerCSharp.CodeAnalysis.Binding;
using CompilerCSharp.CodeAnalysis;
using CompilerCSharp.CodeAnalysis.Syntax;

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
    Binder binder = new Binder();
    BoundExpression boundExpression = binder.BindExpression(syntaxTree.Root);
    IReadOnlyList<string> diagnostics = syntaxTree.Diagnostics;

    if (showTree)
        PrettyPrint(syntaxTree.Root);

    if (diagnostics.Any()){
        foreach(var diagnostic in diagnostics){
            Console.WriteLine(diagnostic);
        }
    } else{
        Evaluator evaluator = new Evaluator(boundExpression);
        Console.WriteLine(evaluator.Evaluate());
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

    indent += isLast ? "   " : "│   ";

    SyntaxNode lastChild = node.GetChildren().LastOrDefault();

    foreach (var child in node.GetChildren()){
        PrettyPrint(child, indent, child == lastChild);
    }
}

