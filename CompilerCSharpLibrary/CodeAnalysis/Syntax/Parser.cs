using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.ExpressionSyntax;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements.Base;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Statements;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий синтаксический анализатор или просто парсер.
    Он строит синтаксическое дерево из токенов, полученных от парсера
    */
    public class Parser{
        private readonly SyntaxToken[] _tokens;
        private int _position;

        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly SourceText _text;

        public DiagnosticBag Diagnostics => _diagnostics;

        //При создании парсера он получает все токены, создавая внутри себя лексер
        public Parser(SourceText text){
            List<SyntaxToken> tokens = new List<SyntaxToken>();

            Lexer lexer = new Lexer(text);
            SyntaxToken token;
            do{
                token = lexer.Lex();

                if (token.Kind != SyntaxKind.WhiteSpaceToken &&
                    token.Kind != SyntaxKind.BadToken){
                        tokens.Add(token);
                    }

            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
            _text = text;
        }

        //Функция для просмотра на offset токенов вперед
        private SyntaxToken Peek(int offset){
            int index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        //Получение текущего токена
        private SyntaxToken Current => Peek(0);

        //Получение текущего токена и переход к следующему
        private SyntaxToken NextToken(){
            SyntaxToken current = Current;
            _position++;
            return current;
        }

        //Если типы токенов совпадают, то возврат текущего и переход к следующему токену
        //Если нет, то создание нового токена
        private SyntaxToken MatchToken(SyntaxKind kind){
            if (Current.Kind == kind)
                return NextToken();

            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        /*
        Метод для постройки синтаксического дерева. Содержит в себе все
        ошибки, само выражение и токен конца файла
        */
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            SyntaxToken endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new CompilationUnitSyntax(statement, endOfFileToken);
        }

        private StatementSyntax ParseStatement(){
            switch (Current.Kind)
            {
                case SyntaxKind.OpenBraceToken:
                    return ParseBlockStatement();
                case SyntaxKind.LetKeyword:
                case SyntaxKind.VarKeyword:
                    return ParseVariableDeclaration();
                case SyntaxKind.IfKeyword:
                    return ParseIfStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private StatementSyntax ParseIfStatement()
        {
            var keyword = MatchToken(SyntaxKind.IfKeyword);
            var condition = ParseExpression();
            var statement = ParseStatement();
            var elseClause = ParseElseClause();
            return new IfStatementSyntax(keyword, condition, statement, elseClause);
        }

        private ElseClauseSyntax ParseElseClause()
        {
            if (Current.Kind != SyntaxKind.ElseKeyword){
                return null;
            }

            var keyword = NextToken();
            var statement = ParseStatement();
            return new ElseClauseSyntax(keyword, statement);
        }

        private StatementSyntax ParseVariableDeclaration()
        {
            var expected = Current.Kind == SyntaxKind.LetKeyword ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword;
            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxKind.IdentifierToken);
            var equals = MatchToken(SyntaxKind.EqualsToken);
            var initializer = ParseExpression();
            return new VariableDeclarationSyntax(keyword, identifier, equals, initializer);
        }

        private ExpressionStatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();
            return new ExpressionStatementSyntax(expression);
        }

        private BlockStatementSyntax ParseBlockStatement()
        {
            var statements = new List<StatementSyntax>();

            var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);

            while (Current.Kind != SyntaxKind.EndOfFileToken &&
            Current.Kind != SyntaxKind.CloseBraceToken){
                var statement = ParseStatement();
                statements.Add(statement);
            }

            var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);
            
            return new BlockStatementSyntax(openBraceToken, statements, closeBraceToken);
        }

        private BaseExpressionSyntax ParseExpression(){
            return ParseAssignmentExpression();
        }

        /*
        Строит дерево для выражения приравнивания.
        Если текущий токен - идентификатор и следующий - =,
        то возвращает объект типа AssignmentExpressionSyntax.
        В обратном случае вызывает дальнейшее построение
        дерева (метод ParseBinaryExpression)
        */
        private BaseExpressionSyntax ParseAssignmentExpression(){
            if (Peek(0).Kind == SyntaxKind.IdentifierToken && 
                Peek(1).Kind == SyntaxKind.EqualsToken){
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            return ParseBinaryExpression();
        }

        /*
        Строит дерево с учетом приоритетов операторов.
        Требуется дальнейшее рассмотрение
        */
        private BaseExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0){
            BaseExpressionSyntax left;

            int unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence){
                SyntaxToken operatorToken = NextToken();
                BaseExpressionSyntax operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            } 
            else {
                left = ParsePrimaryExpression();
            }

            while (true){
                int precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;
                
                SyntaxToken operatorToken = NextToken();
                BaseExpressionSyntax right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        /*
        Функция, проверяющая, является ли текущий токен числом.
        После получения токена возвращает токен NumberExpressionSyntax, 
        хранящий 1 числовой токен
        */
        public BaseExpressionSyntax ParsePrimaryExpression(){
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    return ParseParanthesizedExpression();

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    return ParseBooleanLiteral();

                case SyntaxKind.NumberToken:
                    return ParseNumberLiteral();

                case SyntaxKind.IdentifierToken:
                default:
                    return ParseNameExpression();
            }  
        }

        private BaseExpressionSyntax ParseNumberLiteral()
        {
            SyntaxToken numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }

        private BaseExpressionSyntax ParseParanthesizedExpression()
        {
            SyntaxToken left = MatchToken(SyntaxKind.OpenParenthesisToken);
            BaseExpressionSyntax expression = ParseExpression();
            SyntaxToken right = MatchToken(SyntaxKind.CloseParenthesisToken);
            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        private BaseExpressionSyntax ParseBooleanLiteral()
        {
            bool isTrue = Current.Kind == SyntaxKind.TrueKeyword;
            SyntaxToken keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
            return new LiteralExpressionSyntax(keywordToken, isTrue);
        }

        private BaseExpressionSyntax ParseNameExpression()
        {
            var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
            return new NameExpressionSyntax(identifierToken);
        }
    }
}