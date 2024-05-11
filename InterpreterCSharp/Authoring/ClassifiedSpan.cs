using CompilerCSharpLibrary.CodeAnalysis.Text;

namespace InterpreterCSharp.Authoring
{
    public sealed class ClassifiedSpan
    {
        public ClassifiedSpan(TextSpan span, Classification classification)
        {
            Span = span;
            Classification = classification;
        }

        public TextSpan Span { get; }
        public Classification Classification { get; }
    }
}