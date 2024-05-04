using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий синтаксическое дерево. Содержит в себе
    список зафиксированных ошибок Diagnostics, само выражение Root
    и токен конца файла
    */
    public sealed class SyntaxTree
    {
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

        public static IEnumerable<SyntaxToken> ParseTokens(string text, out DiagnosticBag diagnostics)
        {
            SourceText? sourceText = SourceText.From(text);
            return ParseTokens(sourceText, out diagnostics);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            return ParseTokens(text, out _);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text, out DiagnosticBag diagnostics)
        {
            List<SyntaxToken>? tokens = new List<SyntaxToken>();

            void ParseTokens(SyntaxTree st, out CompilationUnitSyntax root, out DiagnosticBag d)
            {
                root = null;

                Lexer? l = new Lexer(st);
                while (true)
                {
                    SyntaxToken token = l.Lex();
                    if (token.Kind == SyntaxKind.EndOfFileToken)
                    {
                        root = new CompilationUnitSyntax(st, new List<MemberSyntax>(), token);
                        break;
                    }

                    tokens.Add(token);
                }

                d = l.Diagnostics;
            }
            
            SyntaxTree? syntaxTree = new SyntaxTree(text, ParseTokens);
            diagnostics = syntaxTree.Diagnostics;
            return tokens.ToArray();
        }
    }
}