using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий синтаксическое дерево. Содержит в себе
    список зафиксированных ошибок Diagnostics, само выражение Root
    и токен конца файла
    */
    public sealed class SyntaxTree
    {
        private Dictionary<SyntaxNode, SyntaxNode?>? _parents;
        
        private delegate void ParseHandler(SyntaxTree syntaxTree,
                                           out CompilationUnitSyntax root,
                                           out DiagnosticBag diagnostics);
        private SyntaxTree(SourceText text, ParseHandler handler)
        {
            Text = text;

            handler(this, out CompilationUnitSyntax? root, out DiagnosticBag? diagnostics);

            Diagnostics = diagnostics;
            Root = root;
        }

        public SourceText Text { get; }
        public DiagnosticBag Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        public static SyntaxTree Load(string fileName)
        {
            string? text = File.ReadAllText(fileName);
            SourceText? sourceText = SourceText.From(text, fileName);
            return Parse(sourceText);
        }

        private static void Parse(SyntaxTree syntaxTree, out CompilationUnitSyntax root, out DiagnosticBag diagnostics)
        {
            Parser? parser = new Parser(syntaxTree);
            root = parser.ParseCompilationUnit();
            diagnostics = parser.Diagnostics;
        }


        /*
        Мы получаем построенное синтаксическое дерево
        с помощью следующих функций, так как они статические,
        и для них не надо создавать новый объект типа SyntaxTree.
        */
        //Создает парсер и возвращает построенное дерево
        public static SyntaxTree Parse(string text)
        {
            SourceText? sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text, Parse);
        }

        public static List<SyntaxToken> ParseTokens(string text, bool includeEndOfFile = false)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText, includeEndOfFile);
        }

        public static List<SyntaxToken> ParseTokens(string text, out DiagnosticBag diagnostics, bool includeEndOfFile = false)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText, out diagnostics, includeEndOfFile);
        }

        public static List<SyntaxToken> ParseTokens(SourceText text, bool includeEndOfFile = false)
        {
            return ParseTokens(text, out _, includeEndOfFile);
        }

        public static List<SyntaxToken> ParseTokens(SourceText text, out DiagnosticBag diagnostics, bool includeEndOfFile = false)
        {
            var tokens = new List<SyntaxToken>();

            void ParseTokens(SyntaxTree st, out CompilationUnitSyntax root, out DiagnosticBag d)
            {
                var l = new Lexer(st);
                while (true)
                {
                    var token = l.Lex();

                    if (token.Kind != SyntaxKind.EndOfFileToken || includeEndOfFile)
                        tokens.Add(token);

                    if (token.Kind == SyntaxKind.EndOfFileToken)
                    {
                        root = new CompilationUnitSyntax(st, new List<MemberSyntax>(), token);
                        break;
                    }
                }

                d = l.Diagnostics;
            }

            var syntaxTree = new SyntaxTree(text, ParseTokens);
            diagnostics = syntaxTree.Diagnostics;
            return tokens;
        }

        internal SyntaxNode? GetParent(SyntaxNode syntaxNode)
        {
            if (_parents == null)
            {
                var parents = CreateParentsDictionary(Root);
                Interlocked.CompareExchange(ref _parents, parents, null);
            }

            return _parents[syntaxNode];
        }

        private Dictionary<SyntaxNode, SyntaxNode?> CreateParentsDictionary(CompilationUnitSyntax root)
        {
            var result = new Dictionary<SyntaxNode, SyntaxNode?>();
            result.Add(root, null);
            CreateParentsDictionary(result, root);
            return result;
        }

        private void CreateParentsDictionary(Dictionary<SyntaxNode, SyntaxNode?> result, SyntaxNode node)
        {
            foreach (var child in node.GetChildren())
            {
                result.Add(child, node);
                CreateParentsDictionary(result, child);
            }
        }
    }
}