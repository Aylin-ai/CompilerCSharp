namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, содержащий информацию о размере текста,
    о месте его старта и конца
    */
    public class TextSpan{
        public TextSpan(int start, int length){
            Start = start;
            Length = length;
        }

        public int Start { get; }
        public int Length { get; }
        public int End => Start + Length;
    }
}