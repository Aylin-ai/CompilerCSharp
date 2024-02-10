using CompilerCSharp.CodeAnalysis.Syntax;
using CompilerCSharp.CodeAnalysis.Binding;

namespace CompilerCSharp.CodeAnalysis
{
    /*
    Класс, вычисляющий выражение, вводимое в консоли
    */
    class Evaluator{
        public Evaluator(BoundExpression root){
            _root = root;
        }

        private readonly BoundExpression _root;

        public object Evaluate(){
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression n){
                return n.Value;
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
                    
                    default:
                        throw new Exception($"Unexpected binary operator {b.Op.Kind}");
                }
            }

            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}