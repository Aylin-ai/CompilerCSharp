
using System.Reflection;

namespace CompilerCSharpLibrary.CodeAnalysis.Symbols
{
    public static class BuiltInFunctions{
        public static readonly FunctionSymbol Print = new FunctionSymbol(
            "print", 
            new List<ParameterSymbol>() {
                new ParameterSymbol("text", TypeSymbol.String)
            }, 
            TypeSymbol.Void
        );
        public static readonly FunctionSymbol Input = new FunctionSymbol(
            "input", 
            new List<ParameterSymbol>(),
            TypeSymbol.String
        );
        public static readonly FunctionSymbol Rnd = new FunctionSymbol(
            "rnd", 
            new List<ParameterSymbol>(){
                new ParameterSymbol("min", TypeSymbol.Int),
                new ParameterSymbol("max", TypeSymbol.Int)
            },
            TypeSymbol.String
        );

        internal static IEnumerable<FunctionSymbol> GetAll() 
        => typeof(BuiltInFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                                   .Where(f => f.FieldType == typeof(FunctionSymbol))
                                   .Select(f => (FunctionSymbol)f.GetValue(null));
    }
}