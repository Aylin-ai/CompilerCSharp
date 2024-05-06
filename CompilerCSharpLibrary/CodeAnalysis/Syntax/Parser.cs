using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements;
using System.Collections.Generic;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий синтаксический анализатор или просто парсер.
    Он строит синтаксическое дерево из токенов, полученных от парсера
    */
    public class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;

        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly SourceText _text;
        private readonly SyntaxTree _syntaxTree;

        public DiagnosticBag Diagnostics => _diagnostics;

        //При создании парсера он получает все токены, создавая внутри себя лексер
        public Parser(SyntaxTree syntaxTree)
        {
            List<SyntaxToken> tokens = new List<SyntaxToken>();

            Lexer lexer = new Lexer(syntaxTree);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();

                if (token.Kind != SyntaxKind.WhiteSpaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }

            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _syntaxTree = syntaxTree;
            _text = syntaxTree.Text;
            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        //Функция для просмотра на offset токенов вперед
        private SyntaxToken Peek(int offset)
        {
            int index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        //Получение текущего токена
        private SyntaxToken Current => Peek(0);

        //Получение текущего токена и переход к следующему
        private SyntaxToken NextToken()
        {
            SyntaxToken current = Current;
            _position++;
            return current;
        }

        //Если типы токенов совпадают, то возврат текущего и переход к следующему токену
        //Если нет, то создание нового токена
        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            _diagnostics.ReportUnexpectedToken(Current.Location, Current.Kind, kind);
            return new SyntaxToken(_syntaxTree, kind, Current.Position, null, null);
        }

        /*
        Метод для постройки синтаксического дерева. Содержит в себе все
        ошибки, само выражение и токен конца файла
        */
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            List<MemberSyntax>? members = ParseMembers();
            SyntaxToken endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new CompilationUnitSyntax(_syntaxTree, members, endOfFileToken);
        }

        private List<MemberSyntax> ParseMembers()
        {
            List<MemberSyntax>? members = new List<MemberSyntax>();

            while (Current.Kind != SyntaxKind.EndOfFileToken)
            {
                SyntaxToken? startToken = Current;

                MemberSyntax? member = ParseMember();
                members.Add(member);

                if (Current == startToken)
                    NextToken();
            }

            return members;
        }

        private MemberSyntax ParseMember()
        {
            if (Current.Kind == SyntaxKind.FunctionKeyword)
                return ParseFunctionDeclaration();

            return ParseGlobalStatement();
        }

        private MemberSyntax ParseGlobalStatement()
        {
            StatementSyntax? statement = ParseStatement();
            return new GlobalStatementSyntax(_syntaxTree, statement);
        }

        private MemberSyntax ParseFunctionDeclaration()
        {
            SyntaxToken? functionKeyword = MatchToken(SyntaxKind.FunctionKeyword);
            SyntaxToken? identifier = MatchToken(SyntaxKind.IdentifierToken);

            SyntaxToken openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);
            SeparatedSyntaxList<ParameterSyntax>? parameters = ParseParameterList();
            SyntaxToken closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);
            TypeClauseSyntax? type = ParseOptionalTypeClause();
            BlockStatementSyntax? body = ParseBlockStatement();
            return new FunctionDeclarationSyntax(_syntaxTree, functionKeyword, identifier, openParenthesisToken,
            parameters, closeParenthesisToken, type, body);
        }

        private SeparatedSyntaxList<ParameterSyntax> ParseParameterList()
        {
            List<SyntaxNode>? nodesAndSeparators = new List<SyntaxNode>();

            bool parseNextParameter = true;

            while (parseNextParameter &&
                   Current.Kind != SyntaxKind.CloseParenthesisToken &&
                   Current.Kind != SyntaxKind.EndOfFileToken)
            {
                ParameterSyntax? parameter = ParseParameter();
                nodesAndSeparators.Add(parameter);

                if (Current.Kind == SyntaxKind.CommaToken)
                {
                    SyntaxToken? comma = MatchToken(SyntaxKind.CommaToken);
                    nodesAndSeparators.Add(comma);
                }
                else
                {
                    parseNextParameter = false;
                }
            }

            return new SeparatedSyntaxList<ParameterSyntax>(nodesAndSeparators);
        }

        private ParameterSyntax ParseParameter()
        {
            SyntaxToken? identifier = MatchToken(SyntaxKind.IdentifierToken);
            TypeClauseSyntax? type = ParseTypeClause();
            return new ParameterSyntax(_syntaxTree, identifier, type);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.LetKeyword:
                case SyntaxKind.VarKeyword:
                    return ParseVariableDeclaration();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
                case SyntaxKind.WhileKeyword:
                    return ParseWhileStatement();
                case SyntaxKind.ForKeyword:
                    return ParseForStatement();
                case SyntaxKind.DoKeyword:
                    return ParseDoWhileStatement();
                case SyntaxKind.BreakKeyword:
                    return ParseBreakStatement();
                case SyntaxKind.ContinueKeyword:
                    return ParseContinueStatement();
                case SyntaxKind.ReturnKeyword:
                    return ParseReturnStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        /*
        Проблема такова, что по каким-то причинам метод GetLineIndex()
        срабатывает так, что у выражения и return оказываются разные строки (код строки 213, 214).
        Временное решение: добавить -1 к Current.Span.Start (вроде работает)
        */
        private StatementSyntax ParseReturnStatement()
        {
            // return 12;
            // return;

            SyntaxToken? keyword = MatchToken(SyntaxKind.ReturnKeyword);
            bool hasExpression = Current.Kind != SyntaxKind.SemiColonToken;
            BaseExpressionSyntax? expression = null;
            if (hasExpression){
                expression = ParseExpression();
            }
            MatchToken(SyntaxKind.SemiColonToken);
            return new ReturnStatementSyntax(_syntaxTree, keyword, expression);
        }

        private StatementSyntax ParseContinueStatement()
        {
            SyntaxToken? keyword = MatchToken(SyntaxKind.ContinueKeyword);
            return new ContinueStatementSyntax(_syntaxTree, keyword);
        }

        private StatementSyntax ParseBreakStatement()
        {
            SyntaxToken? keyword = MatchToken(SyntaxKind.BreakKeyword);
            return new BreakStatementSyntax(_syntaxTree, keyword);
        }

        private StatementSyntax ParseDoWhileStatement()
        {
            SyntaxToken? doKeyword = MatchToken(SyntaxKind.DoKeyword);
            StatementSyntax? body = ParseStatement();
            SyntaxToken? whileKeyword = MatchToken(SyntaxKind.WhileKeyword);
            BaseExpressionSyntax? condition = ParseExpression();
            return new DoWhileStatementSyntax(_syntaxTree, doKeyword, body, whileKeyword, condition);
        }

        private StatementSyntax ParseForStatement()
        {
            SyntaxToken? keyword = MatchToken(SyntaxKind.ForKeyword);
            SyntaxToken? identifier = MatchToken(SyntaxKind.IdentifierToken);
            SyntaxToken? equalsToken = MatchToken(SyntaxKind.EqualsToken);
            BaseExpressionSyntax? lowerBound = ParseExpression();
            SyntaxToken? toKeyword = MatchToken(SyntaxKind.ToKeyword);
            BaseExpressionSyntax? upperBound = ParseExpression();
            StatementSyntax? body = ParseStatement();
            return new ForStatementSyntax(_syntaxTree, keyword, identifier, equalsToken, lowerBound, toKeyword, upperBound, body);
        }

        private StatementSyntax ParseWhileStatement()
        {
            SyntaxToken? keyword = MatchToken(SyntaxKind.WhileKeyword);
            BaseExpressionSyntax? condition = ParseExpression();
            StatementSyntax? body = ParseStatement();
            return new WhileStatementSyntax(_syntaxTree, keyword, condition, body);
        }

        private StatementSyntax ParseIfStatement()
        {
            SyntaxToken? keyword = MatchToken(SyntaxKind.IfKeyword);
            BaseExpressionSyntax? condition = ParseExpression();
            StatementSyntax? statement = ParseStatement();
            ElseClauseSyntax? elseClause = ParseElseClause();
            return new IfStatementSyntax(_syntaxTree, keyword, condition, statement, elseClause);
        }

        private ElseClauseSyntax ParseElseClause()
        {
            if (Current.Kind != SyntaxKind.ElseKeyword)
            {
                return null;
            }

            SyntaxToken? keyword = NextToken();
            StatementSyntax? statement = ParseStatement();
            return new ElseClauseSyntax(_syntaxTree, keyword, statement);
        }

        private StatementSyntax ParseVariableDeclaration()
        {
            SyntaxKind expected = Current.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword;
            SyntaxToken? keyword = MatchToken(expected);
            SyntaxToken? identifier = MatchToken(SyntaxKind.IdentifierToken);
            TypeClauseSyntax? typeClause = ParseOptionalTypeClause();
            SyntaxToken? equals = MatchToken(SyntaxKind.EqualsToken);
            BaseExpressionSyntax? initializer = ParseExpression();
            return new VariableDeclarationSyntax(_syntaxTree, keyword, identifier, typeClause, equals, initializer);
        }

        private TypeClauseSyntax ParseOptionalTypeClause()
        {
            if (Current.Kind != SyntaxKind.ColonToken)
                return null;

            return ParseTypeClause();
        }

        private TypeClauseSyntax ParseTypeClause()
        {
            SyntaxToken? colonToken = MatchToken(SyntaxKind.ColonToken);
            SyntaxToken? identifier = MatchToken(SyntaxKind.IdentifierToken);
            return new TypeClauseSyntax(_syntaxTree, colonToken, identifier);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            BaseExpressionSyntax? expression = ParseExpression();
            return new ExpressionStatementSyntax(_syntaxTree, expression);
        }

        private BlockStatementSyntax ParseBlockStatement()
        {
            List<StatementSyntax>? statements = new List<StatementSyntax>();

            SyntaxToken? openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);

            while (Current.Kind != SyntaxKind.EndOfFileToken &&
            Current.Kind != SyntaxKind.CloseBraceToken)
            {
                SyntaxToken? startToken = Current;

                StatementSyntax? statement = ParseStatement();
                statements.Add(statement);

                //Пропускаем токен, чтобы избежать бесконечного цикла
                if (Current == startToken)
                {
                    NextToken();
                }
            }

            SyntaxToken? closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);

            return new BlockStatementSyntax(_syntaxTree, openBraceToken, statements, closeBraceToken);
        }

        private BaseExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        /*
        Строит дерево для выражения приравнивания.
        Если текущий токен - идентификатор и следующий - =,
        то возвращает объект типа AssignmentExpressionSyntax.
        В обратном случае вызывает дальнейшее построение
        дерева (метод ParseBinaryExpression)
        */
        private BaseExpressionSyntax ParseAssignmentExpression()
        {
            if (Peek(0).Kind == SyntaxKind.IdentifierToken &&
                Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                SyntaxToken? identifierToken = NextToken();
                SyntaxToken? operatorToken = NextToken();
                BaseExpressionSyntax? right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(_syntaxTree, identifierToken, operatorToken, right);
            }

            return ParseBinaryExpression();
        }

        /*
        Строит дерево с учетом приоритетов операторов.
        Требуется дальнейшее рассмотрение
        */
        private BaseExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            BaseExpressionSyntax left;

            int unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                SyntaxToken operatorToken = NextToken();
                BaseExpressionSyntax operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(_syntaxTree, operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                int precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                SyntaxToken operatorToken = NextToken();
                BaseExpressionSyntax right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(_syntaxTree, left, operatorToken, right);
            }

            return left;
        }

        /*
        Функция, проверяющая, является ли текущий токен числом.
        После получения токена возвращает токен NumberExpressionSyntax, 
        хранящий 1 числовой токен
        */
        public BaseExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    return ParseParanthesizedExpression();

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return ParseBooleanLiteral();

                case SyntaxKind.NumberToken:
                    return ParseNumberLiteral();

                case SyntaxKind.StringToken:
                    return ParseStringLiteral();

                case SyntaxKind.IdentifierToken:
                default:
                    return ParseNameOrCallExpression();
            }
        }

        private BaseExpressionSyntax ParseNumberLiteral()
        {
            SyntaxToken numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(_syntaxTree, numberToken);
        }

        private BaseExpressionSyntax ParseStringLiteral()
        {
            SyntaxToken stringToken = MatchToken(SyntaxKind.StringToken);
            return new LiteralExpressionSyntax(_syntaxTree, stringToken);
        }

        private BaseExpressionSyntax ParseParanthesizedExpression()
        {
            SyntaxToken left = MatchToken(SyntaxKind.OpenParenthesisToken);
            BaseExpressionSyntax expression = ParseExpression();
            SyntaxToken right = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new ParenthesizedExpressionSyntax(_syntaxTree, left, expression, right);
        }

        private BaseExpressionSyntax ParseBooleanLiteral()
        {
            bool isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            SyntaxToken keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(_syntaxTree, keywordToken, isTrue);
        }

        private BaseExpressionSyntax ParseNameOrCallExpression()
        {
            if (Peek(0).Kind == SyntaxKind.IdentifierToken && Peek(1).Kind == SyntaxKind.OpenParenthesisToken)
                return ParseCallExpression();

            return ParseNameExpression();
        }

        private BaseExpressionSyntax ParseCallExpression()
        {
            SyntaxToken? identifier = MatchToken(SyntaxKind.IdentifierToken);
            SyntaxToken? openParenthesisToken = MatchToken(SyntaxKind.OpenParenthesisToken);
            SeparatedSyntaxList<BaseExpressionSyntax>? arguments = ParseArguments();
            SyntaxToken? closeParenthesisToken = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new CallExpressionSyntax(_syntaxTree, identifier, openParenthesisToken, arguments, closeParenthesisToken);
        }

        private SeparatedSyntaxList<BaseExpressionSyntax> ParseArguments()
        {
            List<SyntaxNode>? nodesAndSeparators = new List<SyntaxNode>();

            bool parseNextArgument = true;

            while (parseNextArgument &&
                   Current.Kind != SyntaxKind.CloseParenthesisToken &&
                   Current.Kind != SyntaxKind.EndOfFileToken)
            {
                BaseExpressionSyntax? expression = ParseExpression();
                nodesAndSeparators.Add(expression);

                if (Current.Kind == SyntaxKind.CommaToken)
                {
                    SyntaxToken? comma = MatchToken(SyntaxKind.CommaToken);
                    nodesAndSeparators.Add(comma);
                }
                else
                {
                    parseNextArgument = false;
                }
            }

            return new SeparatedSyntaxList<BaseExpressionSyntax>(nodesAndSeparators);
        }

        private BaseExpressionSyntax ParseNameExpression()
        {
            SyntaxToken? identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(_syntaxTree, identifierToken);
        }

    }
}