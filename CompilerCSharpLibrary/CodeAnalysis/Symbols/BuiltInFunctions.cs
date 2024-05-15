
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CompilerCSharpLibrary.CodeAnalysis.Symbols
{
    public static class BuiltInFunctions
    {

        public static readonly FunctionSymbol Print = new FunctionSymbol(
            "print",
            new List<ParameterSymbol>() {
                new ParameterSymbol("text", TypeSymbol.Any, 0)
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
                new ParameterSymbol("min", TypeSymbol.Int, 0),
                new ParameterSymbol("max", TypeSymbol.Int, 1)
            },
            TypeSymbol.Int
        );

        internal static IEnumerable<FunctionSymbol?> GetAll()
        => typeof(BuiltInFunctions).GetFields(BindingFlags.Public | BindingFlags.Static)
                                   .Where(f => f.FieldType == typeof(FunctionSymbol))
                                   .Select(f => (FunctionSymbol?)f.GetValue(null));
    
    }
}