using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;

namespace CompilerCSharpLibrary.CodeAnalysis.Lowering{
    public sealed class Lowerer : BoundTreeRewriter{
        private Lowerer(){

        }

        public static BoundStatement Lower(BoundStatement statement){
            var lowerer = new Lowerer();
            return lowerer.RewriteStatement(statement);    
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            return base.RewriteForStatement(node);
        }
    }
}