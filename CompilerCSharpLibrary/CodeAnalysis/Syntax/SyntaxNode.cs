using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Абстрактный класс, представляющий узел в синтаксическом дереве.
    От него исходят все остальные классы, включая SyntaxToken.
    Содержит тип токена и метод получения дочерних токенов.
    */
    public abstract class SyntaxNode
    {

        #region Поля класса

        public SyntaxTree SyntaxTree { get; }
        public abstract SyntaxKind Kind { get; }
        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }
        public virtual TextSpan FullSpan
        {
            get
            {
                var first = GetChildren().First().FullSpan;
                var last = GetChildren().Last().FullSpan;
                return TextSpan.FromBounds(first.Start, last.End);
            }
        }
        public TextLocation Location => new TextLocation(SyntaxTree.Text, Span);

        #endregion

        #region Конструкторы класса

        private protected SyntaxNode(SyntaxTree syntaxTree)
        {
            SyntaxTree = syntaxTree;
        }

        #endregion

        #region Методы класса

        public abstract IEnumerable<SyntaxNode> GetChildren();

        public SyntaxToken GetLastToken()
        {
            if (this is SyntaxToken token)
                return token;

            // A syntax node should always contain at least 1 token.
            return GetChildren().Last().GetLastToken();
        }

        public void WriteTo(TextWriter writer)
        {
            PrettyPrint(writer, this);
        }

        private static void PrettyPrint(TextWriter writer, SyntaxNode node, string indent = "", bool isLast = true)
        {
            var isToConsole = writer == Console.Out;
            var token = node as SyntaxToken;

            if (token != null)
            {
                foreach (var trivia in token.LeadingTrivia)
                {
                    if (isToConsole)
                        Console.ForegroundColor = ConsoleColor.DarkGray;

                    writer.Write(indent);
                    writer.Write("├──");

                    if (isToConsole)
                        Console.ForegroundColor = ConsoleColor.DarkGreen;

                    writer.WriteLine($"L: {trivia.Kind}");
                }
            }

            var hasTrailingTrivia = token != null && token.TrailingTrivia.Any();
            var tokenMarker = !hasTrailingTrivia && isLast ? "└──" : "├──";

            if (isToConsole)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            writer.Write(indent);
            writer.Write(tokenMarker);

            if (isToConsole)
                Console.ForegroundColor = node is SyntaxToken ? ConsoleColor.Blue : ConsoleColor.Cyan;

            writer.Write(node.Kind);

            if (token != null && token.Value != null)
            {
                writer.Write(" ");
                writer.Write(token.Value);
            }

            if (isToConsole)
                Console.ResetColor();

            writer.WriteLine();

            if (token != null)
            {
                foreach (var trivia in token.TrailingTrivia)
                {
                    var isLastTrailingTrivia = trivia == token.TrailingTrivia.Last();
                    var triviaMarker = isLast && isLastTrailingTrivia ? "└──" : "├──";

                    if (isToConsole)
                        Console.ForegroundColor = ConsoleColor.DarkGray;

                    writer.Write(indent);
                    writer.Write(triviaMarker);

                    if (isToConsole)
                        Console.ForegroundColor = ConsoleColor.DarkGreen;

                    writer.WriteLine($"T: {trivia.Kind}");
                }
            }

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(writer, child, indent, child == lastChild);
        }

        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                WriteTo(writer);
                return writer.ToString();
            }
        }

        #endregion

    }
}