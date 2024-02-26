using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
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
        private SyntaxTree(SourceText text)
        {

            Parser parser = new Parser(text);
            var root = parser.ParseCompilationUnit();

            Text = text;
            Diagnostics = parser.Diagnostics;
            Root = root;
        }

        public SourceText Text { get; }
        public DiagnosticBag Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        //Создает парсер и возвращает построенное дерево
        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text, out DiagnosticBag diagnostics)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText, out diagnostics);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            return ParseTokens(text, out _);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text, out DiagnosticBag diagnostics)
        {
            IEnumerable<SyntaxToken> LexTokens(Lexer lexer)
            {
                while (true)
                {
                    SyntaxToken token = lexer.Lex();
                    if (token.Kind == SyntaxKind.EndOfFileToken)
                        break;

                    yield return token;
                }
            }
            Lexer l = new Lexer(text);
            var result = LexTokens(l);
            diagnostics = l.Diagnostics;
            return result;
        }
    }
}