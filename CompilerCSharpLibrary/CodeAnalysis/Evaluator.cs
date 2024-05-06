using System;
using System.Collections.Generic;
using CompilerCSharpLibrary.CodeAnalysis.Binding;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis
{
    /*
    Класс, вычисляющий выражение, вводимое в консоли
    */
    public class Evaluator
    {
        private readonly BoundProgram _program;

        //Словарь всех переменных. Ключ - имя переменной, Значение - значение переменной
        private readonly Dictionary<VariableSymbol, object> _globals;
        private readonly Dictionary<FunctionSymbol, BoundBlockStatement> _functions = new Dictionary<FunctionSymbol, BoundBlockStatement>();
        private readonly Stack<Dictionary<VariableSymbol, object>> _locals = new Stack<Dictionary<VariableSymbol, object>>();
        private Random _random;

        private object _lastValue;

        public Evaluator(BoundProgram program, Dictionary<VariableSymbol, object> variables)
        {
            _program = program;
            _globals = variables;
            _locals.Push(new Dictionary<VariableSymbol, object>());

            BoundProgram current = program;
            while (current != null)
            {
                foreach (KeyValuePair<FunctionSymbol, BoundBlockStatement> functionWithBody in current.Functions)
                {
                    FunctionSymbol? function = functionWithBody.Key;
                    BoundBlockStatement? body = functionWithBody.Value; 

                    _functions.Add(function, body);
                }
                current = current.Previous;
            }
        }

        public object Evaluate()
        {
            var function = _program.MainFunction ?? _program.ScriptFunction;
            if (function == null)
                return null;

            var body = _functions[function];
            return EvaluateStatement(body);
        }

        private object EvaluateStatement(BoundBlockStatement body)
        {
            Dictionary<BoundLabel, int>? labelToIndex = new Dictionary<BoundLabel, int>();

            for (int i = 0; i < body.Statements.Count; i++)
            {
                if (body.Statements[i] is BoundLabelStatement l)
                {
                    labelToIndex.Add(l.Label, i + 1);
                }
            }

            int index = 0;

            while (index < body.Statements.Count)
            {

                BoundStatement? s = body.Statements[index];

                switch (s.Kind)
                {
                    case BoundNodeKind.VariableDeclaration:
                        EvaluateVariableDeclaration((BoundVariableDeclaration)s);
                        index++;
                        break;
                    case BoundNodeKind.ExpressionStatement:
                        EvaluateExpressionStatement((BoundExpressionStatement)s);
                        index++;
                        break;
                    case BoundNodeKind.GotoStatement:
                        BoundGotoStatement? gs = (BoundGotoStatement)s;
                        index = labelToIndex[gs.Label];
                        break;
                    case BoundNodeKind.ConditionalGotoStatement:
                        BoundConditionalGotoStatement? cgs = (BoundConditionalGotoStatement)s;
                        bool condition = (bool)EvaluateExpression(cgs.Condition);
                        if (condition == cgs.JumpIfTrue)
                            //Прыжок к нужной строке (индексу)
                            index = labelToIndex[cgs.Label];
                        else
                            index++;
                        break;
                    case BoundNodeKind.LabelStatement:
                        index++;
                        break;
                    case BoundNodeKind.ReturnStatement:
                        BoundReturnStatement? rs = (BoundReturnStatement)s;
                        _lastValue = rs.Expression == null ? null : EvaluateExpression(rs.Expression);
                        return _lastValue;
                    default:
                        throw new Exception($"Unexpected node {s.Kind}");
                }
            }



            return _lastValue;
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
        {
            object? value = EvaluateExpression(node.Initializer);
            _lastValue = value;
            Assign(node.Variable, value);
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

        //Вычисляет выражение, используя построенное АСД
        private object EvaluateExpression(BoundExpression node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeKind.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)node);
                case BoundNodeKind.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeKind.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeKind.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)node);
                case BoundNodeKind.CallExpression:
                    return EvaluateCallExpression((BoundCallExpression)node);
                case BoundNodeKind.ConversionExpression:
                    return EvaluateConversionExpression((BoundConversionExpression)node);
                default:
                    throw new Exception($"Unexpected node {node.Kind}");
            }

        }

        private object EvaluateConversionExpression(BoundConversionExpression node)
        {
            object? value = EvaluateExpression(node.Expression);
            if (node.Type == TypeSymbol.Any)
                return value;
            if (node.Type == TypeSymbol.Bool)
                return Convert.ToBoolean(value);
            else if (node.Type == TypeSymbol.Int)
                return Convert.ToInt32(value);
            else if (node.Type == TypeSymbol.String)
                return Convert.ToString(value);
            else
                throw new Exception($"Unexpected type {node.Type}");
        }

        private object EvaluateCallExpression(BoundCallExpression node)
        {
            if (node.Function == BuiltInFunctions.Input)
            {
                return Console.ReadLine();
            }
            else if (node.Function == BuiltInFunctions.Print)
            {
                string? message = (string)EvaluateExpression(node.Arguments[0]);
                Console.WriteLine(message);
                return null;
            }
            else if (node.Function == BuiltInFunctions.Rnd)
            {
                int minValue = (int)EvaluateExpression(node.Arguments[0]);
                int maxValue = (int)EvaluateExpression(node.Arguments[1]);

                if (_random == null)
                    _random = new Random();

                int randomNumber = _random.Next(minValue, maxValue);
                return randomNumber;
            }
            else
            {
                Dictionary<VariableSymbol, object>? locals = new Dictionary<VariableSymbol, object>();
                for (int i = 0; i < node.Arguments.Count; i++)
                {
                    ParameterSymbol? parameter = node.Function.Parameters[i];
                    object? value = EvaluateExpression(node.Arguments[i]);
                    locals.Add(parameter, value);
                }

                _locals.Push(locals);

                BoundBlockStatement? statement = _functions[node.Function];
                object? result = EvaluateStatement(statement);

                _locals.Pop();

                return result;
            }
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            object left = EvaluateExpression(b.Left);
            object right = EvaluateExpression(b.Right);

            switch (b.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    if (b.Type == TypeSymbol.Int)
                        return (int)left + (int)right;
                    else
                        return (string)left + (string)right;
                case BoundBinaryOperatorKind.Substraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;

                case BoundBinaryOperatorKind.BitwiseAnd:
                    if (b.Type == TypeSymbol.Int)
                        return (int)left & (int)right;
                    else
                        return (bool)left & (bool)right;
                case BoundBinaryOperatorKind.BitwiseOr:
                    if (b.Type == TypeSymbol.Int)
                        return (int)left | (int)right;
                    else
                        return (bool)left | (bool)right;
                case BoundBinaryOperatorKind.BitwiseXor:
                    if (b.Type == TypeSymbol.Int)
                        return (int)left ^ (int)right;
                    else
                        return (bool)left ^ (bool)right;

                case BoundBinaryOperatorKind.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicalOr:
                    return (bool)left || (bool)right;

                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);

                case BoundBinaryOperatorKind.Greater:
                    return (int)left > (int)right;
                case BoundBinaryOperatorKind.GreaterOrEquals:
                    return (int)left >= (int)right;
                case BoundBinaryOperatorKind.Less:
                    return (int)left < (int)right;
                case BoundBinaryOperatorKind.LessOrEquals:
                    return (int)left <= (int)right;

                default:
                    throw new Exception($"Unexpected binary operator {b.Op.Kind}");
            }
        }

        private object EvaluateUnaryExpression(BoundUnaryExpression u)
        {
            object operand = EvaluateExpression(u.Operand);

            switch (u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
                case BoundUnaryOperatorKind.OnesComplement:
                    return ~(int)operand;

                default:
                    throw new Exception($"Unexpected unary operator {u.Op.Kind}");
            }
        }

        /*
        Если происходит приравнивание переменной какого-то
        выражения, то получает результат выражения, приравнивает
        этот результат к переменной и возвращает результат выражения
        */
        private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
        {
            object? value = EvaluateExpression(a.Expression);

            Assign(a.Variable, value);
            
            return value;
        }

        /*
        Если node типа BoundVariableExpression,
        то возвращает значение переменной из списка всех переменных
        */
        private object EvaluateVariableExpression(BoundVariableExpression v)
        {
            if (v.Variable.Kind == SymbolKind.GlobalVariable)
                return _globals[v.Variable];
            else
            {
                Dictionary<VariableSymbol, object>? locals = _locals.Peek();
                return locals[v.Variable];
            }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpression n)
        {
            return n.Value;
        }

        private void Assign(VariableSymbol variable, object value)
        {
            if (variable.Kind == SymbolKind.GlobalVariable)
            {
                _globals[variable] = value;
            }
            else
            {
                Dictionary<VariableSymbol, object>? locals = _locals.Peek();
                locals[variable] = value;
            }
        }
    }
}