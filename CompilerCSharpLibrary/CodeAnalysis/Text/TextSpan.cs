
namespace CompilerCSharpLibrary.CodeAnalysis.Text
{
    /*
    Класс, содержащий информацию о размере текста,
    о месте его старта и конца
    */
    public class TextSpan
    {
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;

        public static TextSpan FromBounds(int start, int end)
        {
            int length = end - start;
            return new TextSpan(start, length);
        }

        public override string ToString() => $"{Start}..{End}";
    }
}