

namespace CompilerCSharpLibrary.CodeAnalysis.Text
{
    /*
    Класс, содержащий информацию о размере текста,
    о месте его старта и конца
    */
    public sealed class SourceText
    {
        private readonly string _text;
        private SourceText(string text, string fileName)
        {
            Lines = ParseLines(this, text);
            _text = text;
            FileName = fileName;
        }

        public List<TextLine> Lines { get; }

        public char this[int index] => _text[index];
        public int Length => _text.Length;

        public string FileName { get; }

        public static SourceText From(string text, string fileName = "")
        {
            return new SourceText(text, fileName);
        }

        public int GetLineIndex(int position)
        {
            int lower = 0;
            int upper = Lines.Count - 1;

            while (lower <= upper)
            {
                int index = lower + (upper - lower) / 2;
                int start = Lines[index].Start;

                if (start == position)
                    return index;

                if (start > position)
                {
                    upper = index - 1;
                }
                else
                {
                    lower = index + 1;
                }
            }

            return lower - 1;
        }

        private static List<TextLine> ParseLines(SourceText sourceText, string text)
        {
            List<TextLine> lines = new List<TextLine>();
            int position = 0;
            int lineStart = 0;
            while (position < text.Length)
            {
                int lineBreakWidth = GetLineBreakWidth(text, position);

                if (lineBreakWidth == 0)
                {
                    position++;
                }
                else
                {
                    AddLine(lines, sourceText, position, lineStart, lineBreakWidth);

                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            if (position >= lineStart)
            {
                AddLine(lines, sourceText, position, lineStart, 0);
            }
            return lines;
        }

        private static void AddLine(List<TextLine> lines, SourceText sourceText, int position, int lineStart, int lineBreakWidth)
        {
            int lineLength = position - lineStart;
            int lineLengthIncludingLineBreak = lineLength + lineBreakWidth;
            TextLine line = new TextLine(sourceText, lineStart, lineLength, lineLengthIncludingLineBreak);
            lines.Add(line);
        }

        private static int GetLineBreakWidth(string text, int position)
        {
            char c = text[position];
            char l = position + 1 >= text.Length ? '\0' : text[position + 1];

            if (c == '\r' && l == '\n')
                return 2;
            if (c == '\r' || l == '\n')
                return 1;

            return 0;
        }

        public override string ToString() => _text;
        public string ToString(int start, int length) => _text.Substring(start, length);
        public string ToString(TextSpan span) => ToString(span.Start, span.Length);
    }
}