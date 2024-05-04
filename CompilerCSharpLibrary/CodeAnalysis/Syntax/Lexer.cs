using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using System.Text;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий лексический анализатор, который 
    находит в тексте токены и возвращает их синтаксическому
    анализатору (парсеру)
    */
    public class Lexer
    {
        private readonly SourceText _text;
        private readonly SyntaxTree _syntaxTree;
        private DiagnosticBag _diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => _diagnostics;

        private int _position;

        private int _start;
        private SyntaxKind _kind;
        private object _value;


        public Lexer(SyntaxTree syntaxTree)
        {
            _syntaxTree = syntaxTree;
            _text = syntaxTree.Text;
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
        public SyntaxToken Lex()
        {
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
                case '{':
                    _kind = SyntaxKind.OpenBraceToken;
                    _position++;
                    break;
                case '}':
                    _kind = SyntaxKind.CloseBraceToken;
                    _position++;
                    break;
                case ',':
                    _kind = SyntaxKind.CommaToken;
                    _position++;
                    break;
                case ':':
                    _kind = SyntaxKind.ColonToken;
                    _position++;
                    break;
                case ';':
                    _kind = SyntaxKind.SemiColonToken;
                    _position++;
                    break;
                case '~':
                    _kind = SyntaxKind.TildeToken;
                    _position++;
                    break;
                case '^':
                    _kind = SyntaxKind.HatToken;
                    _position++;
                    break;
                case '!':
                    if (Lookahead == '=')
                    {
                        _kind = SyntaxKind.NotEqualsToken;
                        _position += 2;
                        break;
                    }
                    _kind = SyntaxKind.BangToken;
                    _position++;
                    break;
                case '&':
                    if (Lookahead == '&')
                    {
                        _kind = SyntaxKind.AmpersandAmpersandToken;
                        _position += 2;
                        break;
                    }
                    _kind = SyntaxKind.AmpersandToken;
                    _position++;
                    break;
                case '>':
                    if (Lookahead == '=')
                    {
                        _kind = SyntaxKind.GreaterOrEqualsToken;
                        _position += 2;
                        break;
                    }
                    _kind = SyntaxKind.GreaterToken;
                    _position++;
                    break;
                case '<':
                    if (Lookahead == '=')
                    {
                        _kind = SyntaxKind.LessOrEqualsToken;
                        _position += 2;
                        break;
                    }
                    _kind = SyntaxKind.LessToken;
                    _position++;
                    break;
                case '|':
                    if (Lookahead == '|')
                    {
                        _kind = SyntaxKind.PipePipeToken;
                        _position += 2;
                        break;
                    }
                    _kind = SyntaxKind.PipeToken;
                    _position++;
                    break;
                case '=':
                    if (Lookahead == '=')
                    {
                        _kind = SyntaxKind.EqualsEqualsToken;
                        _position += 2;
                        break;
                    }
                    _kind = SyntaxKind.EqualsToken;
                    _position++;
                    break;
                case '\"':
                    ReadString();
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
                        var span = new TextSpan(_position, 1);
                        var location = new TextLocation(_text, span);
                        _diagnostics.ReportBadCharacter(location, Current);
                        _position++;
                    }
                    break;


            }

            int length = _position - _start;
            string text = SyntaxFacts.GetText(_kind);
            if (text == null)
                text = _text.ToString(_start, length);

            return new SyntaxToken(_syntaxTree, _kind, _position, text, _value);
        }

        private void ReadString()
        {
            _position++;
            var sb = new StringBuilder();

            var done = false;
            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        var span = new TextSpan(_start, 1);
                        var location = new TextLocation(_text, span);
                        _diagnostics.ReportUnterminatedString(location);
                        done = true;
                        break;
                    case '"':
                        if (Lookahead == '"')
                        {
                            sb.Append(Current);
                            _position += 2;
                        }
                        else
                        {
                            _position++;
                            done = true;
                        }
                        break;
                    default:
                        sb.Append(Current);
                        _position++;
                        break;
                }
            }

            _kind = SyntaxKind.StringToken;
            _value = sb.ToString();
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current) || char.IsDigit(Current))
            {
                _position++;
            }

            int length = _position - _start;
            string text = _text.ToString(_start, length);
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
            string text = _text.ToString(_start, length);

            if (!int.TryParse(text, out int value))
            {
                var span = new TextSpan(_start, length);
                var location = new TextLocation(_text, span);
                _diagnostics.ReportInvalidNumber(location, text, TypeSymbol.Int);
            }

            _value = value;
            _kind = SyntaxKind.NumberToken;
        }
    }
}