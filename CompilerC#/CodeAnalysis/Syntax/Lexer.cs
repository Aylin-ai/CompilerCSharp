namespace CompilerCSharp.CodeAnalysis.Syntax
{
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
        public SyntaxToken Lex(){
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
                
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, null, null);
            }

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
            }

            _diagnostics.Append($"ERROR: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}