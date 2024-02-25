using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using System.Text;

bool showTree = false;
bool showProgram = false;
var variables = new Dictionary<VariableSymbol, object>();
var textBuilder = new StringBuilder();
Compilation previous = null;

while (true){
    if (textBuilder.Length == 0)
        Console.Write("> ");
    else
        Console.Write("| ");

    string input = Console.ReadLine();
    var isBlank = string.IsNullOrWhiteSpace(input);

    if (textBuilder.Length == 0){
        if (isBlank)
            break;
        //Отладочные команды для показа дерева и очистки консоли
        else if (input == "#showTree"){
            showTree = !showTree;
            Console.WriteLine(showTree ? "Showing parse tree" : "Not showing parse tree");
            continue;
        } 
        else if (input == "#showProgram"){
            showProgram = !showProgram;
            Console.WriteLine(showProgram ? "Showing program" : "Not showing program");
            continue;
        } 
        else if (input == "#clear"){
            Console.Clear();
            continue;
        }
        else if (input == "#reset"){
            previous = null;
            continue;
        }
    }

    textBuilder.AppendLine(input);
    var text = textBuilder.ToString();

    SyntaxTree syntaxTree = SyntaxTree.Parse(text);

    if (!isBlank && syntaxTree.Diagnostics.Any()){
        continue;
    }

    Compilation compilation = previous == null ? new Compilation(syntaxTree) : previous.ContinueWith(syntaxTree);


    if (showTree)
        syntaxTree.Root.WriteTo(Console.Out);
    if (showProgram){
        compilation.EmitTree(Console.Out);
    }

    EvaluationResult result = compilation.Evaluate(variables);
    DiagnosticBag diagnostics = result.Diagnostics;

    //Выдает ошибку с некоторыми выражениями (например, 2++)
    if (diagnostics.Any()){
        foreach(var diagnostic in diagnostics){
            int lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
            var lineNumber = lineIndex + 1;
            var line = syntaxTree.Text.Lines[lineIndex];
            var character = diagnostic.Span.Start - line.Start + 1;

            Console.WriteLine();

            Console.Write($"({lineNumber}, {character}): ");
            Console.WriteLine(diagnostic);

            var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
            var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

            string prefix = syntaxTree.Text.ToString(prefixSpan);
            string error = syntaxTree.Text.ToString(diagnostic.Span);
            string suffix = syntaxTree.Text.ToString(suffixSpan);

            Console.Write("    ");
            Console.Write(prefix);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(error);
            Console.ResetColor();

            Console.Write(suffix);

            Console.WriteLine();
        }
        Console.WriteLine();
    } 
    else{
        Console.WriteLine(result.Value);
        previous = compilation;
    }
    textBuilder.Clear();
}

