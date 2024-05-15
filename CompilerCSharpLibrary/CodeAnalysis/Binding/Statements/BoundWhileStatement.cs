using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Statements
{
    public sealed class BoundWhileStatement : BoundLoopStatement
    {
        public BoundWhileStatement(BoundExpression condition,
                                   BoundStatement body,
                                   BoundLabel breakLabel,
                                   BoundLabel continueLabel)
        : base(body, breakLabel, continueLabel)
        {
            Condition = condition;
        }

        public BoundExpression Condition { get; }
        public override BoundNodeKind Kind => BoundNodeKind.WhileStatement;
    }
}