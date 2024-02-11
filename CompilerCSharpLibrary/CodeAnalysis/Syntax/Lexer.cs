namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий лексический анализатор, который 
    находит в тексте токены и возвращает их синтаксическому
    анализатору (парсеру)
    */
    public class Lexer{
        private readonly string _text;
        private int _position;

        private DiagnosticBag _diagnostics = new DiagnosticBag();

        public DiagnosticBag Diagnostics => _diagnostics;

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
        public SyntaxToken Lex(){
            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            int start = _position;

            if (char.IsDigit(Current)){

                while (char.IsDigit(Current)){
                    Next();
                }

                int length = _position - start;
                string text = _text.Substring(start, length);
                
                if(!int.TryParse(text, out int value)){
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, length), text, typeof(int));
                }
                
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current)){

                while (char.IsWhiteSpace(Current)){
                    Next();
                }
                
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, null, null);
            }

            if (char.IsLetter(Current)){

                while (char.IsLetter(Current)){
                    Next();
                }

                int length = _position - start;
                string text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeywordKind(text);
                
                return new SyntaxToken(kind, start, text, null);
            }
            //true, false

            switch (Current){
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);

                case '!':
                    if (Lookahead == '='){
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.NotEqualsToken, start, "!=", null);
                    }  
                    return new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null);
                case '&':
                    if (Lookahead == '&'){
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, start, "&&", null);
                    }
                    break;
                case '|':
                    if (Lookahead == '|'){
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.PipePipeToken, start, "||", null);
                    }
                    break;
                case '=':
                    if (Lookahead == '='){
                        _position += 2;
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
                    }
                    break;
            }

            _diagnostics.ReportBadCharacter(_position, Current);
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}