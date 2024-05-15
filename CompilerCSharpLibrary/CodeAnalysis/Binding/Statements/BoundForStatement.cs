using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Statements
{
    public sealed class BoundForStatement : BoundLoopStatement
    {
        public BoundForStatement(VariableSymbol variable,
                                 BoundExpression lowerBound,
                                 BoundExpression upperBound,
                                 BoundStatement body,
                                 BoundLabel breakLabel,
                                 BoundLabel continueLabel)
        : base(body, breakLabel, continueLabel)
        {
            Variable = variable;
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        public VariableSymbol Variable { get; }
        public BoundExpression LowerBound { get; }
        public BoundExpression UpperBound { get; }
        public override BoundNodeKind Kind => BoundNodeKind.ForStatement;

    }
}