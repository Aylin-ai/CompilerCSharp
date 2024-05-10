namespace CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions
{
    public sealed class BoundConstant
    {
        public BoundConstant(object value)
        {
            Value = value;
        }

        public object Value { get; }
    }
}