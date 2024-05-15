using System.Collections.Generic;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.Statements
{
    public sealed class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(List<BoundStatement> statements)
        {
            Statements = statements;
        }

        public List<BoundStatement> Statements { get; }
        public override BoundNodeKind Kind => BoundNodeKind.BlockStatement;
    }
}