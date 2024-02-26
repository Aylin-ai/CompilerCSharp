/*
Абстрактное синтаксическое дерево требуется для того, чтобы
синтаксическое дерево оставалось неизменяемым.
АСД(AST) является разделением задач.
Синтаксические классы показывают синтаксис языка программирования
наиболее четко чтобы отлавливать ошибки.
АСД нужны для отображения внутреннего состояния компилятора.
Разделение просто упрощает понимание и написание кода.
В данном случае в АСД происходит работа с типами выражений, литерал
и операторов. Мы рассматриваем классы Binding больше с абстрактной точки 
зрения, чем с конкретной, в отличие от классов Syntax (Пример, оператор +. 
В Binding на него есть более подробная информация, когда как в Syntax
все ограничивается его токеном)
АСД нужно, чтобы хранить больше информации, в отличие от
синтаксического дерева, а также чтобы последнее было неизменяемо
*/
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding.BoundScopes
{
    public sealed class BoundScope{
        private Dictionary<string, VariableSymbol> _variables;
        private Dictionary<string, FunctionSymbol> _functions;

        public BoundScope(BoundScope parent){
            Parent = parent;
        }

        public BoundScope Parent { get; }

        public bool TryDeclareVariable(VariableSymbol variable){
            if (_variables == null)
                _variables = new Dictionary<string, VariableSymbol>();

            if (_variables.ContainsKey(variable.Name))
                return false;
            
            _variables.Add(variable.Name, variable);
            return true;
        }

        public bool TryLookupVariable(string name, out VariableSymbol variable){
            variable = null;

            if (_variables != null && _variables.TryGetValue(name, out variable))
                return true;

            if (Parent == null)
                return false;
            
            return Parent.TryLookupVariable(name, out variable);
        }

        public List<VariableSymbol> GetDeclaredVariables(){
            if (_variables == null)
                return new List<VariableSymbol>();
            return _variables.Values.ToList();
        }

        public bool TryDeclareFunction(FunctionSymbol function){
            if (_functions == null)
                _functions = new Dictionary<string, FunctionSymbol>();

            if (_functions.ContainsKey(function.Name))
                return false;
            
            _functions.Add(function.Name, function);
            return true;
        }

        public bool TryLookupFunction(string name, out FunctionSymbol function){
            function = null;

            if (_functions != null && _functions.TryGetValue(name, out function))
                return true;

            if (Parent == null)
                return false;
            
            return Parent.TryLookupFunction(name, out function);
        }

        public List<FunctionSymbol> GetDeclaredFunctions(){
            if (_functions == null)
                return new List<FunctionSymbol>();
            return _functions.Values.ToList();
        }
    }
}