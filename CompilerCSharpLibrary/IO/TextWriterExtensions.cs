using System.CodeDom.Compiler;
using CompilerCSharpLibrary.CodeAnalysis;
using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Text;

namespace CompilerCSharpLibrary.IO
{
    public static class TextWriterExtensions
    {
        public static bool IsConsole(this TextWriter writer)
        {
            if (writer == Console.Out)
                return !Console.IsOutputRedirected;

            if (writer == Console.Error)
                return !Console.IsErrorRedirected && !Console.IsOutputRedirected;

            if (writer is IndentedTextWriter iw && iw.InnerWriter.IsConsole())
                return true;

            return false;
        }

        public static void SetForeground(this TextWriter writer, ConsoleColor color)
        {
            if (writer.IsConsole())
                Console.ForegroundColor = color;
        }

        public static void ResetColor(this TextWriter writer)
        {
            if (writer.IsConsole())
                Console.ResetColor();
        }

        public static void WriteKeyword(this TextWriter writer, SyntaxKind kind)
        {
            writer.WriteKeyword(SyntaxFacts.GetText(kind));
        }
        public static void WriteKeyword(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Blue);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteIdentifier(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkYellow);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteNumber(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Cyan);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteString(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.Magenta);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteSpace(this TextWriter writer)
        {
            writer.WritePunctuation(" ");
        }

        public static void WritePunctuation(this TextWriter writer, SyntaxKind kind)
        {
            writer.WritePunctuation(SyntaxFacts.GetText(kind));
        }

        public static void WritePunctuation(this TextWriter writer, string text)
        {
            writer.SetForeground(ConsoleColor.DarkGray);
            writer.Write(text);
            writer.ResetColor();
        }

        public static void WriteDiagnostics(this TextWriter writer, DiagnosticBag diagnostics)
        {
            foreach (Diagnostic? diagnostic in diagnostics.OrderBy(d => d.Location.FileName)
                                                  .ThenBy(d => d.Location.Span.Start)
                                                  .ThenBy(d => d.Location.Span.Length))
            {
                SourceText? text = diagnostic.Location.Text;
                string? fileName = diagnostic.Location.FileName;
                int startLine = diagnostic.Location.StartLine + 1;
                int startCharacter = diagnostic.Location.StartCharacter + 1;
                int endLine = diagnostic.Location.EndLine + 1;
                int endCharacter = diagnostic.Location.EndCharacter + 1;

                TextSpan? span = diagnostic.Location.Span;
                int lineIndex = text.GetLineIndex(span.Start);
                TextLine? line = text.Lines[lineIndex];
                int lineNumber = lineIndex + 1;
                int character = span.Start - line.Start + 1;

                writer.WriteLine();

                writer.SetForeground(ConsoleColor.DarkRed);
                writer.Write($"{fileName}({startLine},{startCharacter},{endLine},{endCharacter}): ");
                writer.WriteLine(diagnostic);
                writer.ResetColor();

                TextSpan? prefixSpan = TextSpan.FromBounds(line.Start, span.Start);
                TextSpan? suffixSpan = TextSpan.FromBounds(span.End, line.End);

                string? prefix = text.ToString(prefixSpan);
                string? error = text.ToString(span);
                string? suffix = text.ToString(suffixSpan);

                writer.Write("    ");
                writer.Write(prefix);

                writer.SetForeground(ConsoleColor.DarkRed);
                writer.Write(error);
                writer.ResetColor();

                writer.Write(suffix);

                writer.WriteLine();
            }

            writer.WriteLine();
        }
    }
}