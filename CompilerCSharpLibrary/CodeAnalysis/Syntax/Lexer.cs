using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using System.Text;
using CompilerCSharpLibrary.CodeAnalysis.Symbols;
using System.Collections.Generic;

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
        private List<SyntaxTrivia> _triviaList = new List<SyntaxTrivia>();


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
            ReadTrivia(leading: true);

            var leadingTrivia = _triviaList;
            var tokenStart = _position;

            ReadToken();

            var tokenKind = _kind;
            var tokenValue = _value;
            var tokenLength = _position - _start;

            ReadTrivia(leading: false);

            var trailingTrivia = _triviaList;

            var tokenText = SyntaxFacts.GetText(tokenKind);
            if (tokenText == null)
                tokenText = _text.ToString(tokenStart, tokenLength);

            return new SyntaxToken(_syntaxTree, tokenKind, tokenStart, tokenText, tokenValue, leadingTrivia, trailingTrivia);
        }

        private void ReadTrivia(bool leading)
        {
            _triviaList.Clear();

            var done = false;

            while (!done)
            {
                _start = _position;
                _kind = SyntaxKind.BadTokenTrivia;
                _value = null;

                switch (Current)
                {
                    case '\0':
                        done = true;
                        break;
                    case '/':
                        if (Lookahead == '/')
                        {
                            ReadSingleLineComment();
                        }
                        else if (Lookahead == '*')
                        {
                            ReadMultiLineComment();
                        }
                        else
                        {
                            done = true;
                        }
                        break;
                    case '\n':
                    case '\r':
                        if (!leading)
                            done = true;
                        ReadLineBreak();
                        break;
                    case ' ':
                    case '\t':
                        ReadWhiteSpace();
                        break;
                    default:
                        if (char.IsWhiteSpace(Current))
                            ReadWhiteSpace();
                        else
                            done = true;
                        break;
                }

                var length = _position - _start;
                if (length > 0)
                {
                    var text = _text.ToString(_start, length);
                    var trivia = new SyntaxTrivia(_syntaxTree, _kind, _start, text);
                    _triviaList.Add(trivia);
                }
            }
        }

        private void ReadLineBreak()
        {
            if (Current == '\r' && Lookahead == '\n')
            {
                _position += 2;
            }
            else
            {
                _position++;
            }

            _kind = SyntaxKind.LineBreakTrivia;
        }

        private void ReadWhiteSpace()
        {
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        done = true;
                        break;
                    default:
                        if (!char.IsWhiteSpace(Current))
                            done = true;
                        else
                            _position++;
                        break;
                }
            }

            _kind = SyntaxKind.WhiteSpaceTrivia;
        }


        private void ReadSingleLineComment()
        {
            _position += 2;
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        done = true;
                        break;
                    default:
                        _position++;
                        break;
                }
            }

            _kind = SyntaxKind.SingleLineCommentTrivia;
        }

        private void ReadMultiLineComment()
        {
            _position += 2;
            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                        var span = new TextSpan(_start, 2);
                        var location = new TextLocation(_text, span);
                        _diagnostics.ReportUnterminatedMultiLineComment(location);
                        done = true;
                        break;
                    case '*':
                        if (Lookahead == '/')
                        {
                            _position++;
                            done = true;
                        }
                        _position++;
                        break;
                    default:
                        _position++;
                        break;
                }
            }

            _kind = SyntaxKind.MultiLineCommentTriva;
        }

        private void ReadToken()
        {
            _start = _position;
            _kind = SyntaxKind.BadTokenTrivia;
            _value = null;

            switch (Current)
            {
                case '\0':
                    _kind = SyntaxKind.EndOfFileToken;
                    break;
                case '+':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.PlusToken;
                    }
                    break;
                case '-':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.MinusToken;
                    }
                    break;
                case '*':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.StarToken;
                    }
                    break;
                case '/':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.SlashToken;
                    }
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
                case ':':
                    _kind = SyntaxKind.ColonToken;
                    _position++;
                    break;
                case ',':
                    _kind = SyntaxKind.CommaToken;
                    _position++;
                    break;
                case '~':
                    _kind = SyntaxKind.TildeToken;
                    _position++;
                    break;
                case '^':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.HatToken;
                    }
                    break;
                case '&':
                    _position++;
                    if (Current == '&')
                    {
                        _kind = SyntaxKind.AmpersandAmpersandToken;
                        _position++;
                    }
                    else
                    {
                        _kind = SyntaxKind.AmpersandToken;
                    }
                    break;
                case '|':
                    _position++;
                    if (Current == '|')
                    {
                        _kind = SyntaxKind.PipePipeToken;
                        _position++;
                    }
                    else
                    {
                        _kind = SyntaxKind.PipeToken;
                    }
                    break;
                case '=':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.EqualsToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.EqualsEqualsToken;
                        _position++;
                    }
                    break;
                case '!':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.BangToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.NotEqualsToken;
                        _position++;
                    }
                    break;
                case '<':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.LessToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.LessOrEqualsToken;
                        _position++;
                    }
                    break;
                case '>':
                    _position++;
                    if (Current != '=')
                    {
                        _kind = SyntaxKind.GreaterToken;
                    }
                    else
                    {
                        _kind = SyntaxKind.GreaterOrEqualsToken;
                        _position++;
                    }
                    break;
                case '"':
                    ReadString();
                    break;
                case '0': case '1': case '2': case '3': case '4':
                case '5': case '6': case '7': case '8': case '9':
                    ReadNumberToken();
                    break;
                case '_':
                    ReadIdentifierOrKeyword();
                    break;
                default:
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeyword();
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
        }

        private void ReadString()
        {
            _position++;
            StringBuilder? sb = new StringBuilder();

            bool done = false;
            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        TextSpan span = new TextSpan(_start, 1);
                        TextLocation location = new TextLocation(_text, span);
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
             while (char.IsLetterOrDigit(Current) || Current == '_')
            {
                _position++;
            }

            int length = _position - _start;
            string text = _text.ToString(_start, length);
            _kind = SyntaxFacts.GetKeywordKind(text);
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
                TextSpan span = new TextSpan(_start, length);
                TextLocation location = new TextLocation(_text, span);
                _diagnostics.ReportInvalidNumber(location, text, TypeSymbol.Int);
            }

            _value = value;
            _kind = SyntaxKind.NumberToken;
        }
    }
}