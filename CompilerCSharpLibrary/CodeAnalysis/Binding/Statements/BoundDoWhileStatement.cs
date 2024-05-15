using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Statements
{
    public sealed class BoundDoWhileStatement : BoundLoopStatement
    {
        public BoundDoWhileStatement(BoundStatement body,
                                     BoundExpression condition,
                                     BoundLabel breakLabel,
                                     BoundLabel continueLabel)
        : base(body, breakLabel, continueLabel)
        {
            Condition = condition;
        }

        public BoundExpression Condition { get; }
        public override BoundNodeKind Kind => BoundNodeKind.DoWhileStatement;
    }
}