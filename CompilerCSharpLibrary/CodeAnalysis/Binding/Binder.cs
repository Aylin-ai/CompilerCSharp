using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundExpressions.Base;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundOperators;
using CompilerCSharpLibrary.CodeAnalysis.Binding.BoundScopes;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Binding.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Lowering;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Text;

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
namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    public sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly FunctionSymbol _function;
        private Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)> _loopStack = new Stack<(BoundLabel BreakLabel, BoundLabel ContinueLabel)>();
        private int _labelCounter;
        private BoundScope _scope;

        public Binder(BoundScope parent, FunctionSymbol function)
        {
            _scope = new BoundScope(parent);
            _function = function;

            if (function != null)
            {
                foreach (ParameterSymbol? p in function.Parameters)
                    _scope.TryDeclareVariable(p);
            }
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, List<SyntaxTree> syntaxTrees)
        {
            BoundScope? parentScope = CreateParentScopes(previous);
            Binder? binder = new Binder(parentScope, function: null);

            IEnumerable<FunctionDeclarationSyntax>? functionDeclarations = syntaxTrees.SelectMany(st => st.Root.Members).OfType<FunctionDeclarationSyntax>();

            foreach (FunctionDeclarationSyntax? function in functionDeclarations)
            {
                binder.BindFunctionDeclaration(function);
            }

            IEnumerable<GlobalStatementSyntax>? globalStatements = syntaxTrees.SelectMany(st => st.Root.Members).OfType<GlobalStatementSyntax>();

            List<BoundStatement>? statements = new List<BoundStatement>();

            foreach (GlobalStatementSyntax? globalStatement in globalStatements)
            {
                BoundStatement? s = binder.BindStatement(globalStatement.Statement);
                statements.Add(s);
            }

            List<FunctionSymbol>? functions = binder._scope.GetDeclaredFunctions();
            List<VariableSymbol>? variables = binder._scope.GetDeclaredVariables();
            DiagnosticBag? diagnostics = binder.Diagnostics;

            if (previous != null)
            {
                diagnostics.AddRange(previous.Diagnostics);
            }

            return new BoundGlobalScope(previous, diagnostics, functions, variables, statements);
        }

        public static BoundProgram BindProgram(BoundGlobalScope globalScope)
        {
            BoundScope? parentScope = CreateParentScopes(globalScope);

            Dictionary<FunctionSymbol, BoundBlockStatement>? functionBodies = new Dictionary<FunctionSymbol, BoundBlockStatement>();
            DiagnosticBag? diagnostics = new DiagnosticBag();
            diagnostics.AddRange(globalScope.Diagnostics);

            BoundGlobalScope? scope = globalScope;

            while (scope != null)
            {
                foreach (FunctionSymbol? function in scope.Functions)
                {
                    //У каждой функции свой Binder, в котором они раскрываются
                    Binder? binder = new Binder(parentScope, function);
                    BoundStatement? body = binder.BindStatement(function.Declaration.Body);
                    BoundBlockStatement? loweredBody = Lowerer.Lower(body);

                    if (function.Type != TypeSymbol.Void && !ControlFlowGraph.AllPathsReturn(loweredBody))
                    {
                        binder._diagnostics.ReportAllPathMustReturn(function.Declaration.Identifier.Location);
                    }

                    functionBodies.Add(function, loweredBody);

                    diagnostics.AddRange(binder.Diagnostics);
                }

                scope = scope.Previous;
            }

            BoundBlockStatement? statement = Lowerer.Lower(new BoundBlockStatement(globalScope.Statements));

            return new BoundProgram(diagnostics, functionBodies, statement);
        }

        private static BoundScope CreateParentScopes(BoundGlobalScope previous)
        {
            Stack<BoundGlobalScope>? stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope parent = CreateRootScope();

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                BoundScope? scope = new BoundScope(parent);
                foreach (FunctionSymbol? f in previous.Functions)
                {
                    scope.TryDeclareFunction(f);
                }
                foreach (VariableSymbol? v in previous.Variables)
                {
                    scope.TryDeclareVariable(v);
                }

                parent = scope;
            }

            return parent;
        }

        private static BoundScope CreateRootScope()
        {
            BoundScope? result = new BoundScope(null);

            foreach (FunctionSymbol? f in BuiltInFunctions.GetAll())
            {
                result.TryDeclareFunction(f);
            }

            return result;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private void BindFunctionDeclaration(FunctionDeclarationSyntax syntax)
        {
            List<ParameterSymbol>? parameters = new List<ParameterSymbol>();

            HashSet<string>? seenParameterNames = new HashSet<string>();

            foreach (ParameterSyntax? parameterSyntax in syntax.Parameters)
            {
                string? parameterName = parameterSyntax.Identifier.Text;
                TypeSymbol? parameterType = BindTypeClause(parameterSyntax.Type);
                if (!seenParameterNames.Add(parameterName))
                {
                    _diagnostics.ReportParameterAlreadyDeclared(parameterSyntax.Location, parameterName);
                }
                else
                {
                    ParameterSymbol? parameter = new ParameterSymbol(parameterName, parameterType);
                    parameters.Add(parameter);
                }
            }

            TypeSymbol? type = BindTypeClause(syntax.Type) ?? TypeSymbol.Void;

            FunctionSymbol? function = new FunctionSymbol(syntax.Identifier.Text, parameters, type, syntax);
            if (function.Declaration.Identifier.Text != null && !_scope.TryDeclareFunction(function))
            {
                _diagnostics.ReportSymbolAlreadyDeclared(syntax.Identifier.Location, function.Name);
            }
        }

        private BoundStatement BindErrorStatement()
        {
            return new BoundExpressionStatement(new BoundErrorExpression());
        }

        private BoundStatement BindStatement(StatementSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.BlockStatement:
                    return BindBlockStatement((BlockStatementSyntax)syntax);
                case SyntaxKind.VariableDeclaration:
                    return BindVariableDeclaration((VariableDeclarationSyntax)syntax);
                case SyntaxKind.ExpressionStatement:
                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);
                case SyntaxKind.IfStatement:
                    return BindIfStatement((IfStatementSyntax)syntax);
                case SyntaxKind.WhileStatement:
                    return BindWhileStatement((WhileStatementSyntax)syntax);
                case SyntaxKind.DoWhileStatement:
                    return BindDoWhileStatement((DoWhileStatementSyntax)syntax);
                case SyntaxKind.ForStatement:
                    return BindForStatement((ForStatementSyntax)syntax);
                case SyntaxKind.BreakStatement:
                    return BindBreakStatement((BreakStatementSyntax)syntax);
                case SyntaxKind.ContinueStatement:
                    return BindContinueStatement((ContinueStatementSyntax)syntax);
                case SyntaxKind.ReturnStatement:
                    return BindReturnStatement((ReturnStatementSyntax)syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundStatement BindReturnStatement(ReturnStatementSyntax syntax)
        {
            BoundExpression? expression = syntax.Expression == null ? null : BindExpression(syntax.Expression);

            if (_function == null)
                _diagnostics.ReportInvalidReturn(syntax.ReturnKeyword.Location);
            else
            {
                if (_function.Type == TypeSymbol.Void)
                {
                    if (expression != null)
                    {
                        _diagnostics.ReportInvalidReturnExpression(syntax.Expression.Location, _function.Name);
                    }
                }
                else
                {
                    if (expression == null)
                        _diagnostics.ReportMissingReturnExpression(syntax.ReturnKeyword.Location, _function.Type);
                    else
                        expression = BindConversion(syntax.Expression.Location, expression, _function.Type);
                }
            }


            return new BoundReturnStatement(expression);
        }

        private BoundStatement BindContinueStatement(ContinueStatementSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.ReportInvalidBreakOrContinue(syntax.Keyword.Location, syntax.Keyword.Text);
                return BindErrorStatement();
            }

            BoundLabel? continueLabel = _loopStack.Peek().ContinueLabel;
            return new BoundGotoStatement(continueLabel);
        }

        private BoundStatement BindBreakStatement(BreakStatementSyntax syntax)
        {
            if (_loopStack.Count == 0)
            {
                _diagnostics.ReportInvalidBreakOrContinue(syntax.Keyword.Location, syntax.Keyword.Text);
                return BindErrorStatement();
            }

            BoundLabel? breakLabel = _loopStack.Peek().BreakLabel;
            return new BoundGotoStatement(breakLabel);
        }

        private BoundStatement BindDoWhileStatement(DoWhileStatementSyntax syntax)
        {
            BoundStatement? body = BindLoopBody(syntax.Body, out BoundLabel? breakLabel, out BoundLabel? continueLabel);
            BoundExpression? condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            return new BoundDoWhileStatement(body, condition, breakLabel, continueLabel);
        }

        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            BoundExpression? lowerBound = BindExpression(syntax.LowerBound, TypeSymbol.Int);
            BoundExpression? upperBound = BindExpression(syntax.UpperBound, TypeSymbol.Int);

            _scope = new BoundScope(_scope);

            VariableSymbol variable = BindVariableDeclaration(syntax.Identifier, isReadOnly: true, TypeSymbol.Int);
            BoundStatement? body = BindLoopBody(syntax.Body, out BoundLabel? breakLabel, out BoundLabel? continueLabel);

            _scope = _scope.Parent;

            return new BoundForStatement(variable, lowerBound, upperBound, body, breakLabel, continueLabel);
        }

        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            BoundExpression? condition = BindExpression(syntax.Condition);
            BoundStatement body = BindLoopBody(syntax.Body, out BoundLabel? breakLabel, out BoundLabel? continueLabel);
            return new BoundWhileStatement(condition, body, breakLabel, continueLabel);
        }

        /*
        breakLabel и continueLabel нужны и у каждого цикла только в одном экземпляре
        (один номер), потому что если перевести код в более низкоуровневый, то все
        сведется к goto, при котором в пределах одного цикла будет идти возврат
        только к указанным break и continue
        */
        private BoundStatement BindLoopBody(StatementSyntax body, out BoundLabel breakLabel, out BoundLabel continueLabel)
        {
            _labelCounter++;
            breakLabel = new BoundLabel($"break{_labelCounter}");
            continueLabel = new BoundLabel($"continue{_labelCounter}");

            _loopStack.Push((breakLabel, continueLabel));

            BoundStatement? boundBody = BindStatement(body);
            _loopStack.Pop();

            return boundBody;
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            BoundExpression? condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            BoundStatement? thenStatement = BindStatement(syntax.ThenStatement);
            BoundStatement? elseStatement = syntax.ElseClause == null ? null : BindStatement(syntax.ElseClause.ElseStatement);
            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        //Разобраться
        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            bool isReadOnly = syntax.Keyword.Kind == SyntaxKind.LetKeyword;
            TypeSymbol? type = BindTypeClause(syntax.TypeClause);
            BoundExpression? initializer = BindExpression(syntax.Initializer);
            TypeSymbol? variableType = type ?? initializer.Type;
            VariableSymbol? variable = BindVariableDeclaration(syntax.Identifier, isReadOnly, variableType);
            BoundExpression? convertedInitializer = BindConversion(syntax.Initializer.Location, initializer, variableType);

            return new BoundVariableDeclaration(variable, convertedInitializer);
        }

        private TypeSymbol BindTypeClause(TypeClauseSyntax syntax)
        {
            if (syntax == null)
                return null;

            TypeSymbol? type = LookupType(syntax.Identifier.Text);
            if (type == null)
            {
                _diagnostics.ReportUndefinedType(syntax.Identifier.Location, syntax.Identifier.Text);
            }

            return type;
        }

        private BoundBlockStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            List<BoundStatement>? statements = new List<BoundStatement>();
            _scope = new BoundScope(_scope);
            foreach (StatementSyntax? statementSyntax in syntax.Statements)
            {
                BoundStatement? statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            _scope = _scope.Parent;

            return new BoundBlockStatement(statements);
        }

        private BoundExpressionStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            BoundExpression? expression = BindExpression(syntax.Expression, canBeVoid: true);
            return new BoundExpressionStatement(expression);
        }

        private BoundExpression BindExpression(BaseExpressionSyntax syntax, bool canBeVoid = false)
        {
            BoundExpression? result = BindExpressionInternal(syntax);
            if (!canBeVoid && result.Type == TypeSymbol.Void)
            {
                _diagnostics.ReportExpressionMustHaveValue(syntax.Location);
                return new BoundErrorExpression();
            }

            return result;
        }

        //Возвращает нужный вид выражение с приведением его параметра к нужному типу
        private BoundExpression BindExpressionInternal(BaseExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.ParethesizedExpression:
                    return BindParethesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                case SyntaxKind.CallExpression:
                    return BindCallExpression((CallExpressionSyntax)syntax);

                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindExpression(BaseExpressionSyntax syntax, TypeSymbol targetType)
        {
            return BindConversion(syntax, targetType);
        }

        private BoundExpression BindCallExpression(CallExpressionSyntax syntax)
        {
            if (syntax.Arguments.Count == 1 && LookupType(syntax.Identifier.Text) is TypeSymbol type)
            {
                return BindConversion(syntax.Arguments[0], type);
            }
            List<BoundExpression> boundArguments = new List<BoundExpression>();

            foreach (BaseExpressionSyntax? argument in syntax.Arguments)
            {
                BoundExpression? boundArgument = BindExpression(argument);
                boundArguments.Add(boundArgument);
            }

            //if (!_scope.TryLookupFunction(syntax.Identifier.Text, out var function))
            Symbol? symbol = _scope.TryLookupSymbol(syntax.Identifier.Text);
            if (symbol == null)
            {
                _diagnostics.ReportUndefinedFunction(syntax.Identifier.Location, syntax.Identifier.Text);
                return new BoundErrorExpression();
            }

            FunctionSymbol? function = symbol as FunctionSymbol;
            if (function == null)
            {
                _diagnostics.ReportNotAFunction(syntax.Identifier.Location, syntax.Identifier.Text);
                return new BoundErrorExpression();
            }

            if (syntax.Arguments.Count != function.Parameters.Count)
            {
                TextSpan span;
                if (syntax.Arguments.Count > function.Parameters.Count)
                {
                    SyntaxNode firstExceedingNode;
                    if (function.Parameters.Count > 0)
                        firstExceedingNode = syntax.Arguments.GetSeparator(function.Parameters.Count - 1);
                    else
                        firstExceedingNode = syntax.Arguments[0];
                    BaseExpressionSyntax? lastExceedingArgument = syntax.Arguments[syntax.Arguments.Count - 1];
                    span = TextSpan.FromBounds(firstExceedingNode.Span.Start, lastExceedingArgument.Span.End);
                }
                else
                {
                    span = syntax.CloseParenthesisToken.Span;
                }
                TextLocation location = new TextLocation(syntax.SyntaxTree.Text, span);
                _diagnostics.ReportWrongArgumentCount(location, function.Name, function.Parameters.Count, syntax.Arguments.Count);
                return new BoundErrorExpression();
            }

            bool hasErrors = false;

            for (int i = 0; i < syntax.Arguments.Count; i++)
            {
                BoundExpression? argument = boundArguments[i];
                ParameterSymbol? parameter = function.Parameters[i];
                if (argument.Type != parameter.Type)
                {
                    if (argument.Type != TypeSymbol.Error)
                        _diagnostics.ReportWrongArgumentType(syntax.Arguments[i].Location, parameter.Name, parameter.Type, argument.Type);
                    hasErrors = true;
                }
            }

            if (hasErrors)
                return new BoundErrorExpression();

            return new BoundCallExpression(function, boundArguments);
        }

        //Централизация
        private BoundExpression BindConversion(BaseExpressionSyntax syntax, TypeSymbol type)
        {
            BoundExpression? expression = BindExpression(syntax);
            return BindConversion(syntax.Location, expression, type);
        }

        private BoundExpression BindConversion(TextLocation diagnosticLocation, BoundExpression expression, TypeSymbol type)
        {
            Conversion? conversion = Conversion.Classify(expression.Type, type);

            if (!conversion.Exist)
            {
                if (expression.Type != TypeSymbol.Error && type != TypeSymbol.Error)
                {
                    _diagnostics.ReportCannotConvert(diagnosticLocation, expression.Type, type);
                }

                return new BoundErrorExpression();
            }

            if (conversion.IsIdentity)
            {
                return expression;
            }

            return new BoundConversionExpression(type, expression);
        }

        /*
        Метод, срабатывающий при приравнивании переменной к значению. Получает
        имя переменной, рассматривает выражение. Если такая переменная уже
        существует, то удаляет ее из списка и пересоздает по новой. Если
        такой переменной нет, то просто создает её. В конце возвращает
        узел с информацией по переменной и выражению BoundAssignmentExpression
        */
        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            string? name = syntax.IdentifierToken.Text;
            BoundExpression? boundExpression = BindExpression(syntax.Expression);

            VariableSymbol? variable = BindVariableReference(syntax.IdentifierToken);
            if (variable == null)
                return boundExpression;

            if (variable.IsReadOnly)
                _diagnostics.ReportCannotAssign(syntax.EqualsToken.Location, name);

            BoundExpression? convertedExpression = BindConversion(syntax.Expression.Location, boundExpression, variable.Type);

            return new BoundAssignmentExpression(variable, convertedExpression);
        }
        /*
        Метод, срабатывающий при вызове переменной. Получает ее имя и ищет среди
        всех переменных. Если находит, то возвращает узел с информацией
        по этой переменной BoundVariableExpression. Если нет, то ловит ошибку
        и возвращает узел с литералом, равным 0
        */
        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            string? name = syntax.IdentifierToken.Text;
            if (syntax.IdentifierToken.IsMissing)
                return new BoundErrorExpression();

            //if (!_scope.TryLookupVariable(name, out var variable))
            //{
            //    _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
            //    return new BoundErrorExpression();
            //}
            VariableSymbol? variable = BindVariableReference(syntax.IdentifierToken);
            if (variable == null)
                return new BoundErrorExpression();

            return new BoundVariableExpression(variable);
        }

        //Реализация выражений в скобках
        private BoundExpression BindParethesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        /*
        Возвращает объект класса BindBinaryExpression,
        данные для которого получены из синтаксического 
        поддерева BinaryExpressionSyntax
        */
        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            BoundExpression boundLeft = BindExpression(syntax.Left);
            BoundExpression boundRight = BindExpression(syntax.Right);

            if (boundLeft.Type == TypeSymbol.Error ||
                boundRight.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            BoundBinaryOperator boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return new BoundErrorExpression();
            }
            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }

        /*
        Возвращает объект класса BoundUnaryExpression,
        данные для которого получены из синтаксического 
        поддерева UnaryExpressionSyntax
        */
        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            BoundExpression boundOperand = BindExpression(syntax.Operand);

            if (boundOperand.Type == TypeSymbol.Error)
                return new BoundErrorExpression();

            BoundUnaryOperator boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Location, syntax.OperatorToken.Text, boundOperand.Type);
                return new BoundErrorExpression();
            }
            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        /*
        Возвращает объект класса BoundLiteralExpression,
        содержащий значение, его тип и вид выражения,
        полученные из синтаксического поддерева LiteralExpressionSyntax
        */
        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            object value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }

        public VariableSymbol BindVariableDeclaration(SyntaxToken identifier, bool isReadOnly, TypeSymbol type)
        {
            string? name = identifier.Text ?? "?";
            bool declare = identifier.IsMissing;
            //Если мы внутри функции (function != null), то локальная переменная. Иначе - глобальная.
            VariableSymbol? variable = _function == null
            ? (VariableSymbol)new GlobalVariableSymbol(name, isReadOnly, type)
            : new LocalVariableSymbol(name, isReadOnly, type);

            bool isDeclared = _scope.TryDeclareVariable(variable);
            if (declare && !isDeclared)
                _diagnostics.ReportSymbolAlreadyDeclared(identifier.Location, name);
            return variable;
        }

        private VariableSymbol BindVariableReference(SyntaxToken identifierToken)
        {
            string? name = identifierToken.Text;
            switch (_scope.TryLookupSymbol(name))
            {
                case VariableSymbol variable:
                    return variable;

                case null:
                    _diagnostics.ReportUndefinedVariable(identifierToken.Location, name);
                    return null;

                default:
                    _diagnostics.ReportNotAVariable(identifierToken.Location, name);
                    return null;
            }
        }

        private TypeSymbol LookupType(string name)
        {
            switch (name)
            {
                case "bool":
                    return TypeSymbol.Bool;
                case "int":
                    return TypeSymbol.Int;
                case "string":
                    return TypeSymbol.String;
                default:
                    return null;
            }
        }
    }
}