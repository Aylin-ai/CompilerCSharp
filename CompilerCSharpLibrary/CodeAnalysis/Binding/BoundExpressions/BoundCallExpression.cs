using System.Collections.Generic;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions
{
    public sealed class BoundCallExpression : BoundExpression
    {
        public BoundCallExpression(FunctionSymbol function,
                                   List<BoundExpression> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        public FunctionSymbol Function { get; }
        public List<BoundExpression> Arguments { get; }
        public override TypeSymbol Type => Function.Type;
        public override BoundNodeKind Kind => BoundNodeKind.CallExpression;
    }
}