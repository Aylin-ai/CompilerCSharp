using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Statements
{
    public sealed class BoundIfStatement : BoundStatement
    {
        public BoundIfStatement(BoundExpression condition,
                                BoundStatement thenStatement,
                                BoundStatement elseStatement)
        {
            Condition = condition;
            ThenStatement = thenStatement;
            ElseStatement = elseStatement;
        }

        public BoundExpression Condition { get; }
        public BoundStatement ThenStatement { get; }
        public BoundStatement ElseStatement { get; }
        public override BoundNodeKind Kind => BoundNodeKind.IfStatement;
    }
}