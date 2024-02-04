using CompilerCSharp.CodeAnalysis;

bool showTree = false;

while (true){
    Console.Write("> ");
    string line = Console.ReadLine();
    if (string.IsNullOrEmpty(line))
        return;

    if (line == "#showTree"){
        showTree = !showTree;
        Console.WriteLine(showTree ? "Showing parse tree" : "Not showing parse tree");
        continue;
    } else if (line == "#clear"){
        Console.Clear();
        continue;
    }

    SyntaxTree syntaxTree = SyntaxTree.Parse(line);

    if (showTree)
        PrettyPrint(syntaxTree.Root);

    if (syntaxTree.Diagnostics.Any()){
        foreach(var diagnostic in syntaxTree.Diagnostics){
            Console.WriteLine(diagnostic);
        }
    } else{
        Evaluator evaluator = new Evaluator(syntaxTree.Root);
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

    indent += isLast ? "    " : "│   ";

    SyntaxNode lastChild = node.GetChildren().LastOrDefault();

    foreach (var child in node.GetChildren()){
        PrettyPrint(child, indent, child == lastChild);
    }
}

