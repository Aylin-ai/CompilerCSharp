


while (true){
    Console.Write("> ");
    string line = Console.ReadLine();
    if (string.IsNullOrEmpty(line))
        return;

    Parser parser = new Parser(line);
    SyntaxTree syntaxTree = parser.Parse();

    PrettyPrint(syntaxTree.Root);



    if (syntaxTree.Diagnostics.Any()){
        foreach(var diagnostic in syntaxTree.Diagnostics){
            Console.WriteLine(diagnostic);
        }
    } else{
        Evaluator evaluator = new Evaluator(syntaxTree.Root);
        Console.WriteLine(evaluator.Evaluate());
    }
}

/*
Функция, выводящая синтаксическое дерево. Выводит данные
о токенах. В зависимости от того, последний это элемент в дереве
или нет выводит 3 символа, указанных вначале функции
*/
static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true){
    //├──
    //│
    //└──

    string marker = isLast ? "└──" : "├──";


    Console.Write(indent);
    Console.Write(marker);
    Console.Write(node.Kind);

    if (node is SyntaxToken t && t.Value != null){
        Console.Write($" {t.Value}");
    }
    Console.WriteLine();

    indent += isLast ? "    " : "│   ";

    SyntaxNode lastChild = node.GetChildren().LastOrDefault();

    foreach (var child in node.GetChildren()){
        PrettyPrint(child, indent, child == lastChild);
    }
}

//Виды токенов
enum SyntaxKind{
    NumberToken,
    WhiteSpaceToken,
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    OpenParenthesisToken,
    CloseParenthesisToken,
    BadToken,
    EndOfFileToken,
    NumberExpression,
    BinaryExpression
}

/*
Класс, представляющий токен и все его атрибуты: тип, позиция в тексте,
лексема и значение, если есть. У данного токена нет дочерних токенов
*/
class SyntaxToken : SyntaxNode{
    public SyntaxToken(SyntaxKind kind, int position, string text, object value){
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }

    public override SyntaxKind Kind { get; }

    public int Position { get; }
    public string Text { get; }
    public object Value { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Enumerable.Empty<SyntaxNode>();
    }
}

/*
Класс, представляющий лексический анализатор, который 
находит в тексте токены и возвращает их синтаксическому
анализатору (парсеру)
*/
class Lexer{
    private readonly string _text;
    private int _position;

    private List<string> _diagnostics = new List<string>();

    public IEnumerable<string> Diagnostics => _diagnostics;

    public Lexer(string text){
        _text = text;
    }

    /*
    Возращает текущий символ или последний символ в тексте, 
    в зависимости от значения _position
    */
    private char Current {
        get{
            if (_position >= _text.Length)
                return '\0';

            return _text[_position];
        }
    }

    //Переход на следующий символ
    private void Next(){
        _position++;
    }

    /*
     Функция, возвращающая токен. Если _position в конце или за концом текста,
     то возвращает токен EndOfFileToken.
     Если повстречалась цифра, то отмечает это место первого появления.
     После проходит вперед по символам до тех пор, пока не перестанут появляться
     цифры. После вычисляет длину лексемы, вычитая из текущей позиции отмеченную нами.
     После этого пытается перевести данную лексему в значение типа int. После этого
     создает токен и возвращает его.
     Такая же логика и с пробельными символами.
     И в случае спец. символов, указанных в функции возвращает их токены, вместе
     с этим переходя на символ вперед.
     В противном случае возвращает токен типа BadToken.
    */
    public SyntaxToken NextToken(){
        if (_position >= _text.Length)
            return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

        if (char.IsDigit(Current)){
            int start = _position;

            while (char.IsDigit(Current)){
                Next();
            }

            int length = _position - start;
            string text = _text.Substring(start, length);
            
            if(!int.TryParse(text, out int value)){
                _diagnostics.Add($"ERROR: The number {text} cannot be represented as Int32");
            }
            
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }

        if (char.IsWhiteSpace(Current)){
            int start = _position;

            while (char.IsWhiteSpace(Current)){
                Next();
            }

            int length = _position - start;
            return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, null, null);
        }

        if (Current == '+'){
            return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
        } else if (Current == '-'){
            return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
        } else if (Current == '*'){
            return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
        } else if (Current == '/'){
            return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
        } else if (Current == '('){
            return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
        } else if (Current == ')'){
            return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
        }

        _diagnostics.Append($"ERROR: bad character input: '{Current}'");
        return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
    }
}

/*
Абстрактный класс, представляющий узел в синтаксическом дереве.
От него исходят все остальные классы, включая SyntaxToken.
Содержит тип токена и метод получения дочерних токенов.
*/
abstract class SyntaxNode{
    public abstract SyntaxKind Kind { get;}

    public abstract IEnumerable<SyntaxNode> GetChildren();
}

/*
Абстрактный класс, представляющий узел выражения,
от которого идет реализация конкретных узлов выражения, по типу
NumberExpression или BinaryExpression
*/
abstract class ExpressionSyntax : SyntaxNode{

}

/*
Класс, представляющий узел числа, от которого идет
узел, представляющий числовой токен.
В качестве дочерних узлов содержит один узел,
представляющий число
*/
sealed class NumberExpressionSyntax : ExpressionSyntax{
    public NumberExpressionSyntax(SyntaxToken numberToken){
        NumberToken = numberToken;
    }

    public override SyntaxKind Kind => SyntaxKind.NumberExpression;

    public SyntaxToken NumberToken { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NumberToken;
    }
}

/*
Класс, представляющий узел, от которого исходит 3 узла,
являющиеся левым операндом, оператором и правым операндом
бинарного выражения.
Левые и правые операнды могут ветвиться дальше, то есть
сами являться деревьями, т.к. класса ExpressionSyntax.
*/
sealed class BinaryExpressionSyntax : ExpressionSyntax{
    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right){
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public ExpressionSyntax Left { get; }
    public SyntaxToken OperatorToken { get; }
    public ExpressionSyntax Right { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}

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
            token = lexer.NextToken();

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
    private SyntaxToken Match(SyntaxKind kind){
        if (Current.Kind == kind)
            return NextToken();

        _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
        return new SyntaxToken(kind, Current.Position, null, null);
    }

    /*
    Метод
    */
    public SyntaxTree Parse()
    {
        ExpressionSyntax expression = ParsTerm();
        SyntaxToken endOfFileToken = Match(SyntaxKind.EndOfFileToken);
        return new SyntaxTree(Diagnostics, expression, endOfFileToken);
    }

    
    private ExpressionSyntax ParsTerm()
    {
        //Получает токен NumberExpressionSyntax левого операнда бинарного выражения
        ExpressionSyntax left = ParseFactor();

        //Пока текущий токен равен плюсу или минусу, делать...
        while (Current.Kind == SyntaxKind.PlusToken ||
               Current.Kind == SyntaxKind.MinusToken)
        {
            //Получение текущего токена - оператора, и переход к следующему
            SyntaxToken operatorToken = NextToken();
            ////Получает токен NumberExpressionSyntax правого операнда бинарного выражения
            ExpressionSyntax right = ParseFactor();
            //В качестве левого операнда создает новое бинарное выражения,
            //в качестве значений которого берет сам себя, найденный оператор и правое бинарное выражение
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        //Возвращает все выражение
        return left;
    }

    private ExpressionSyntax ParseFactor()
    {
        //Получает токен NumberExpressionSyntax левого операнда бинарного выражения
        ExpressionSyntax left = ParsePrimaryExpression();

        //Пока текущий токен равен плюсу или минусу, делать...
        while (Current.Kind == SyntaxKind.StarToken ||
               Current.Kind == SyntaxKind.SlashToken)
        {
            //Получение текущего токена - оператора, и переход к следующему
            SyntaxToken operatorToken = NextToken();
            ////Получает токен NumberExpressionSyntax правого операнда бинарного выражения
            ExpressionSyntax right = ParsePrimaryExpression();
            //В качестве левого операнда создает новое бинарное выражения,
            //в качестве значений которого берет сам себя, найденный оператор и правое бинарное выражение
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        //Возвращает все выражение
        return left;
    }

    /*
    Функция, проверяющая, является ли текущий токен числом.
    После получения токена возвращает токен NumberExpressionSyntax, 
    хранящий 1 числовой токен
    */
    public ExpressionSyntax ParsePrimaryExpression(){
        SyntaxToken numberToken = Match(SyntaxKind.NumberToken);
        return new NumberExpressionSyntax(numberToken);
    }
}

sealed class SyntaxTree{
    public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken){
        Diagnostics = diagnostics.ToArray();
        Root = root;
        EndOfFileToken = endOfFileToken;
    }

    public IReadOnlyList<string> Diagnostics { get; }
    public ExpressionSyntax Root { get; }
    public SyntaxToken EndOfFileToken { get; }
}

class Evaluator{
    public Evaluator(ExpressionSyntax root){
        _root = root;
    }

    private readonly ExpressionSyntax _root;

    public int Evaluate(){
        return EvaluateExpression(_root);
    }

    private int EvaluateExpression(ExpressionSyntax node)
    {
        if (node is NumberExpressionSyntax n){
            return (int) n.NumberToken.Value;
        }
        if (node is BinaryExpressionSyntax b){
            int left = EvaluateExpression(b.Left);
            int right = EvaluateExpression(b.Right);

            if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                return left + right;
            else if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                return left - right;
            else if (b.OperatorToken.Kind == SyntaxKind.StarToken)
                return left * right;
            else if (b.OperatorToken.Kind == SyntaxKind.SlashToken)
                return left / right;
            else
                throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
        }
        throw new Exception($"Unexpected node {node.Kind}");
    }
}