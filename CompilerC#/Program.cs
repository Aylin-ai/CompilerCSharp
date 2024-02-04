

while (true){
    Console.Write("> ");
    string line = Console.ReadLine();
    if (string.IsNullOrEmpty(line))
        return;

    Parser parser = new Parser(line);
    ExpressionSyntax expression = parser.Parse();

    PrettyPrint(expression);
}

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

class Lexer{
    private readonly string _text;
    private int _position;

    public Lexer(string text){
        _text = text;
    }

    private char Current {
        get{
            if (_position >= _text.Length)
                return '\0';

            return _text[_position];
        }
    }

    private void Next(){
        _position++;
    }

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
            int.TryParse(text, out int value);
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
        }

        if (char.IsWhiteSpace(Current)){
            int start = _position;

            while (char.IsWhiteSpace(Current)){
                Next();
            }

            int length = _position - start;
            string text = _text.Substring(start, length);
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

        return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
    }
}

abstract class SyntaxNode{
    public abstract SyntaxKind Kind { get;}

    public abstract IEnumerable<SyntaxNode> GetChildren();
}

abstract class ExpressionSyntax : SyntaxNode{

}

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

class Parser{
    private readonly SyntaxToken[] _tokens;
    private int _position;

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
    }

    private SyntaxToken Peek(int offset){
        int index = _position + offset;
        if (index >= _tokens.Length)
            return _tokens[_tokens.Length - 1];

        return _tokens[index];
    }

    private SyntaxToken Current => Peek(0);

    private SyntaxToken NextToken(){
        SyntaxToken current = Current;
        _position++;
        return current;
    }

    private SyntaxToken Match(SyntaxKind kind){
        if (Current.Kind == kind)
            return NextToken();

        return new SyntaxToken(kind, Current.Position, null, null);
    }

    public ExpressionSyntax Parse(){
        ExpressionSyntax left = ParsePrimaryExpression();

        while (Current.Kind == SyntaxKind.PlusToken ||
               Current.Kind == SyntaxKind.MinusToken){
            SyntaxToken operatorToken = NextToken();
            ExpressionSyntax right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);    
        }

        return left;
    }

    public ExpressionSyntax ParsePrimaryExpression(){
        SyntaxToken numberToken = Match(SyntaxKind.NumberToken);
        return new NumberExpressionSyntax(numberToken);
    }
}