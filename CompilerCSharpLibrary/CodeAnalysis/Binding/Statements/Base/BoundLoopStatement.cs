namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base
{
    public abstract class BoundLoopStatement : BoundStatement
    {
        protected BoundLoopStatement(BoundStatement body,
                                     BoundLabel breakLabel,
                                     BoundLabel continueLabel)
        {
            Body = body;
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
        }

        public BoundStatement Body { get; }
        public BoundLabel BreakLabel { get; }
        public BoundLabel ContinueLabel { get; }
    }

}