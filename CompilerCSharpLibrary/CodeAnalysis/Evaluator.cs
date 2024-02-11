using CompilerCSharpLibrary.CodeAnalysis.Binding;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, вычисляющий выражение, вводимое в консоли
    */
    public class Evaluator{
        private readonly BoundExpression _root;
        //Словарь всех переменных. Ключ - имя переменной, Значение - значение переменной
        private readonly Dictionary<VariableSymbol, object> _variables;
        
        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables){
            _root = root;
            _variables = variables;
        }

        public object Evaluate(){
            return EvaluateExpression(_root);
        }

        //Вычисляет выражение, используя построенное АСД
        private object EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression n){
                return n.Value;
            }
            /*
            Если node типа BoundVariableExpression,
            то возвращает значение переменной из списка всех переменных
            */
            if (node is BoundVariableExpression v){
                return _variables[v.Variable];
            }
            /*
            Если происходит приравнивание переменной какого-то
            выражения, то получает результат выражения, приравнивает
            этот результат к переменной и возвращает результат выражения
            */
            if (node is BoundAssignmentExpression a){
                var value = EvaluateExpression(a.Expression);
                _variables[a.Variable] = value;
                return value;
            }
            if (node is BoundUnaryExpression u){
                object operand = EvaluateExpression(u.Operand);

                switch (u.Op.Kind){
                    case BoundUnaryOperatorKind.Identity:
                        return (int)operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -(int)operand;
                    case BoundUnaryOperatorKind.LogicalNegation:
                        return !(bool)operand;
                    
                    default:
                        throw new Exception($"Unexpected unary operator {u.Op.Kind}");
                }
            }
            if (node is BoundBinaryExpression b){
                object left = EvaluateExpression(b.Left);
                object right = EvaluateExpression(b.Right);

                switch (b.Op.Kind){
                    case BoundBinaryOperatorKind.Addition:
                        return (int)left + (int)right;
                    case BoundBinaryOperatorKind.Substraction:
                        return (int)left - (int)right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return (int)left * (int)right;
                    case BoundBinaryOperatorKind.Division:
                        return (int)left / (int)right;

                    case BoundBinaryOperatorKind.LogicalAnd:
                        return (bool)left && (bool)right;
                    case BoundBinaryOperatorKind.LogicalOr:
                        return (bool)left || (bool)right;
                    case BoundBinaryOperatorKind.Equals:
                        return Equals(left, right);
                    case BoundBinaryOperatorKind.NotEquals:
                        return !Equals(left, right);
                    
                    default:
                        throw new Exception($"Unexpected binary operator {b.Op.Kind}");
                }
            }

            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}