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
        private Dictionary<string, VariableSymbol> _variables = new Dictionary<string, VariableSymbol>();

        public BoundScope(BoundScope parent){
            Parent = parent;
        }

        public BoundScope Parent { get; }

        public bool TryDeclare(VariableSymbol variable){
            if (_variables.ContainsKey(variable.Name))
                return false;
            
            _variables.Add(variable.Name, variable);
            return true;
        }

        public bool TryLookup(string name, out VariableSymbol variable){
            if (_variables.TryGetValue(name, out variable))
                return true;

            if (Parent == null)
                return false;
            
            return Parent.TryLookup(name, out variable);
        }

        public List<VariableSymbol> GetDeclaredVariables(){
            return _variables.Values.ToList();
        }
    }
}