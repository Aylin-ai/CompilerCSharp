namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий лексический анализатор, который 
    находит в тексте токены и возвращает их синтаксическому
    анализатору (парсеру)
    */
    public class Lexer{
        private readonly string _text;
        private DiagnosticBag _diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => _diagnostics;

        private int _position;

        private int _start;
        private SyntaxKind _kind;
        private object _value;


        public Lexer(string text){
            _text = text;
        }

        /*
        Возращает текущий символ или последний символ в тексте, 
        в зависимости от значения _position
        */
        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            int index = _position + offset;

            if (index >= _text.Length)
                return '\0';

            return _text[index];
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
        public SyntaxToken Lex(){
            _start = _position;
            _kind = SyntaxKind.BadToken;
            _value = null;

            switch (Current)
            {
                    case '\0':
                        _kind = SyntaxKind.EndOfFileToken;
                        break;
                    case '+':
                        _kind = SyntaxKind.PlusToken;
                        _position++;
                        break;
                    case '-':
                        _kind = SyntaxKind.MinusToken;
                        _position++;
                        break;
                    case '*':
                        _kind = SyntaxKind.StarToken;
                        _position++;
                        break;
                    case '/':
                        _kind = SyntaxKind.SlashToken;
                        _position++;
                        break;
                    case '(':
                        _kind = SyntaxKind.OpenParenthesisToken;
                        _position++;
                        break;
                    case ')':
                        _kind = SyntaxKind.CloseParenthesisToken;
                        _position++;
                        break;
                    case '!':
                        if (Lookahead == '='){
                            _kind = SyntaxKind.NotEqualsToken;
                            _position += 2;
                            break;
                        }
                        _kind = SyntaxKind.BangToken;
                        _position++;
                        break;
                    case '&':
                        if (Lookahead == '&'){
                            _kind = SyntaxKind.AmpersandAmpersandToken;
                            _position += 2;
                            break;
                        }
                        break;
                    case '|':
                        if (Lookahead == '|'){
                            _kind = SyntaxKind.PipePipeToken;
                            _position += 2;
                            break;
                        }
                        break;
                    case '=':
                        if (Lookahead == '='){
                            _kind = SyntaxKind.EqualsEqualsToken;
                            _position += 2;
                            break;
                        }
                        _kind = SyntaxKind.EqualsToken;
                        _position++;
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        ReadNumberToken();
                        break;
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        ReadWhiteSpaceToken();
                        break;
                    default:
                        if (char.IsLetter(Current))
                        {
                            ReadIdentifierOrKeyword();
                        }
                        else if (char.IsWhiteSpace(Current))
                        {
                            ReadWhiteSpaceToken();
                        }
                        else
                        {
                            _diagnostics.ReportBadCharacter(_position, Current);
                            _position++;
                        }
                        break;

                       
            }

            int length = _position - _start;
            string text = SyntaxFacts.GetText(_kind);
            if (text == null)
                text = _text.Substring(_start, length);

            return new SyntaxToken(_kind, _position, text, _value);
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current) || char.IsDigit(Current))
            {
                _position++;
            }

            int length = _position - _start;
            string text = _text.Substring(_start, length);
            _kind = SyntaxFacts.GetKeywordKind(text);
        }

        private void ReadWhiteSpaceToken()
        {
            while (char.IsWhiteSpace(Current))
            {
                _position++;
            }

            _kind = SyntaxKind.WhiteSpaceToken;
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
            {
                _position++;
            }

            int length = _position - _start;
            string text = _text.Substring(_start, length);

            if (!int.TryParse(text, out int value))
            {
                _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));
            }

            _value = value;
            _kind = SyntaxKind.NumberToken;
        }
    }
}