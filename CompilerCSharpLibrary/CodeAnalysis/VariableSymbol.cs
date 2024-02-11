namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, представляющий переменную. Содержит имя и тип переменной
    */
    public sealed class VariableSymbol{
        internal VariableSymbol(string name, Type type){
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public Type Type { get; }
    }
}