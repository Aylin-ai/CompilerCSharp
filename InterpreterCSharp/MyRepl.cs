using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;
using static CompilerCSharpLibrary.IO.TextWriterExtensions;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace InterpreterCSharp
{
    public sealed class MyRepl : Repl
    {
        private bool _loadingSubmission;
        private static readonly Compilation emptyCompilation = Compilation.CreateScript(null);

        private Compilation _previous;
        private bool _showTree;
        private bool _showProgram;
        private readonly Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();

        public MyRepl()
        {
            LoadSubmissions();
        }

        private sealed class RenderState
        {
            public RenderState(SourceText text, List<SyntaxToken> tokens)
            {
                Text = text;
                Tokens = tokens;
            }

            public SourceText Text { get; }
            public List<SyntaxToken> Tokens { get; }
        }

        protected override object RenderLine(IReadOnlyList<string> lines, int lineIndex, object state)
        {
            RenderState renderState;

            if (state == null)
            {
                var text = string.Join(Environment.NewLine, lines);
                var sourceText = SourceText.From(text);
                var tokens = SyntaxTree.ParseTokens(sourceText);
                renderState = new RenderState(sourceText, tokens);
            }
            else
            {
                renderState = (RenderState) state;
            }

            var lineSpan = renderState.Text.Lines[lineIndex].Span;

            foreach (var token in renderState.Tokens)
            {
                if (!lineSpan.OverlapsWith(token.Span))
                    continue;

                var tokenStart = Math.Max(token.Span.Start, lineSpan.Start);
                var tokenEnd = Math.Min(token.Span.End, lineSpan.End);
                var tokenSpan = TextSpan.FromBounds(tokenStart, tokenEnd);
                var tokenText = renderState.Text.ToString(tokenSpan);

                var isKeyword = token.Kind.IsKeyword();
                var isIdentifier = token.Kind == SyntaxKind.IdentifierToken;
                var isNumber = token.Kind == SyntaxKind.NumberToken;
                var isString = token.Kind == SyntaxKind.StringToken;
                var isComment = token.Kind.IsComment();

                if (isKeyword)
                    Console.ForegroundColor = ConsoleColor.Blue;
                else if (isIdentifier)
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                else if (isNumber)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                else if (isString)
                    Console.ForegroundColor = ConsoleColor.Magenta;
                else if (isComment)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.Write(tokenText);

                Console.ResetColor();
            }

            return renderState;
        }

        [MetaCommand("exit", "Exits the REPL")]
        private void EvaluateExit()
        {
            Environment.Exit(0);
        }

        [MetaCommand("cls", "Clears the screen")]
        private void EvaluateCls()
        {
            Console.Clear();
        }

        [MetaCommand("reset", "Clears all previous submissions")]
        private void EvaluateReset()
        {
            _previous = null;
            _variables.Clear();
            ClearSubmissions();
        }

        [MetaCommand("showTree", "Shows the parse tree")]
        private void EvaluateShowTree()
        {
            _showTree = !_showTree;
            Console.WriteLine(_showTree ? "Showing parse trees." : "Not showing parse trees.");
        }

        [MetaCommand("showProgram", "Shows the bound tree")]
        private void EvaluateShowProgram()
        {
            _showProgram = !_showProgram;
            Console.WriteLine(_showProgram ? "Showing bound tree." : "Not showing bound tree.");
        }

        [MetaCommand("load", "Loads a script file")]
        private void EvaluateLoad(string path)
        {
            path = Path.GetFullPath(path);

            if (!File.Exists(path))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"error: file does not exist '{path}'");
                Console.ResetColor();
                return;
            }

            string? text = File.ReadAllText(path);
            EvaluateSubmission(text);
        }

        [MetaCommand("ls", "Lists all symbols")]
        private void EvaluateLs()
        {
            Compilation? compilation = _previous ?? emptyCompilation;
            IOrderedEnumerable<Symbol>? symbols = compilation.GetSymbols().OrderBy(s => s.Kind).ThenBy(s => s.Name);
            foreach (Symbol? symbol in symbols)
            {
                symbol.WriteTo(Console.Out);
                Console.WriteLine();
            }
        }

        [MetaCommand("dump", "Shows bound tree of a given function")]
        private void EvaluateDump(string functionName)
        {
            Compilation? compilation = _previous ?? emptyCompilation;
            FunctionSymbol? symbol = compilation.GetSymbols().OfType<FunctionSymbol>().SingleOrDefault(f => f.Name == functionName);

            if (symbol == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"error: function '{functionName}' does not exist");
                Console.ResetColor();
                return;
            }

            compilation.EmitTree(symbol, Console.Out);
        }

        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            bool lastTwoLinesAreBlank = text.Split(Environment.NewLine)
                                           .Reverse()
                                           .TakeWhile(s => string.IsNullOrEmpty(s))
                                           .Take(2)
                                           .Count() == 2;
            if (lastTwoLinesAreBlank)
                return true;

            SyntaxTree? syntaxTree = SyntaxTree.Parse(text);

            // Use Members because we need to exclude the EndOfFileToken.
            var lastMember = syntaxTree.Root.Members.LastOrDefault();
            if (lastMember == null || lastMember.GetLastToken().IsMissing)
                return false;

            return true;
        }

        protected override void EvaluateSubmission(string text)
        {
            SyntaxTree? syntaxTree = SyntaxTree.Parse(text);

            Compilation? compilation = Compilation.CreateScript(_previous, syntaxTree);

            if (_showTree)
                syntaxTree.Root.WriteTo(Console.Out);

            if (_showProgram)
                compilation.EmitTree(Console.Out);

            EvaluationResult? result = compilation.Evaluate(_variables);

            if (!result.ErrorDiagnostics.Any())
            {
                if (result.Value != null)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(result.Value);
                    Console.ResetColor();
                }
                _previous = compilation;

                SaveSubmission(text);
            }
            else
            {
                Console.Out.WriteDiagnostics(result.Diagnostics);
            }
        }

        private static string GetSubmissionsDirectory()
        {
            string? localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string? submissionsDirectory = Path.Combine(localAppData, "Minsk", "Submissions");
            return submissionsDirectory;
        }

        private void LoadSubmissions()
        {
            string? submissionsDirectory = GetSubmissionsDirectory();
            if (!Directory.Exists(submissionsDirectory))
                return;

            string[]? files = Directory.GetFiles(submissionsDirectory).OrderBy(f => f).ToArray();
            if (files.Length == 0)
                return;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"Loaded {files.Length} submission(s)");
            Console.ResetColor();

            _loadingSubmission = true;

            foreach (string? file in files)
            {
                string? text = File.ReadAllText(file);
                EvaluateSubmission(text);
            }

            _loadingSubmission = false;
        }

        private static void ClearSubmissions()
        {
            string? dir = GetSubmissionsDirectory();
            if (Directory.Exists(dir))
                Directory.Delete(dir, recursive: true);
        }

        private void SaveSubmission(string text)
        {
            if (_loadingSubmission)
                return;

            string? submissionsDirectory = GetSubmissionsDirectory();
            Directory.CreateDirectory(submissionsDirectory);
            int count = Directory.GetFiles(submissionsDirectory).Length;
            string? name = $"submission{count:0000}";
            string? fileName = Path.Combine(submissionsDirectory, name);
            File.WriteAllText(fileName, text);
        }
    }
}



