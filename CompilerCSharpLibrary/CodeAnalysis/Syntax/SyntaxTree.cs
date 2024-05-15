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
    и текст с кодом
    */
    public sealed class SyntaxTree
    {

        #region Поля класса

        public SourceText Text { get; }
        public DiagnosticBag Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        #endregion

        #region Конструкторы класса
        
        private delegate void ParseHandler(SyntaxTree syntaxTree,
                                           out CompilationUnitSyntax root,
                                           out DiagnosticBag diagnostics);
        private SyntaxTree(SourceText text,
                           ParseHandler handler)
        {
            Text = text;

            handler(this, out CompilationUnitSyntax? root, out DiagnosticBag? diagnostics);

            Diagnostics = diagnostics;
            Root = root;
        }

        #endregion

        #region Методы класса

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
    
        #endregion
    
    }
}