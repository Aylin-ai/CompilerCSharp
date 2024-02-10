namespace CompilerCSharp.CodeAnalysis
{
    /*
    Класс, представляющий синтаксический анализатор или просто парсер.
    Он строит синтаксическое дерево из токенов, полученных от парсера
    */
    class Parser{
        private readonly SyntaxToken[] _tokens;
        private int _position;

        private List<string> _diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => _diagnostics;

        //При создании парсера он получает все токены, создавая внутри себя лексер
        public Parser(string text){
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

            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        /*
        Метод для постройки синтаксического дерева. Содержит в себе все
        ошибки, само выражение и токен конца файла
        */
        public SyntaxTree Parse()
        {
            ExpressionSyntax expression = ParseExpression();
            SyntaxToken endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(Diagnostics, expression, endOfFileToken);
        }

        /*
        Строит дерево с учетом приоритетов операторов.
        Требуется дальнейшее рассмотрение
        */
        private ExpressionSyntax ParseExpression(int parentPrecedence = 0){
            ExpressionSyntax left = ParsePrimaryExpression();

            while (true){
                int precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;
                
                SyntaxToken operatorToken = NextToken();
                ExpressionSyntax right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        /*
        Функция, проверяющая, является ли текущий токен числом.
        После получения токена возвращает токен NumberExpressionSyntax, 
        хранящий 1 числовой токен
        */
        public ExpressionSyntax ParsePrimaryExpression(){
            if (Current.Kind == SyntaxKind.OpenParenthesisToken){
                SyntaxToken left = NextToken();
                ExpressionSyntax expression = ParseExpression();
                SyntaxToken right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            SyntaxToken numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
    }
}