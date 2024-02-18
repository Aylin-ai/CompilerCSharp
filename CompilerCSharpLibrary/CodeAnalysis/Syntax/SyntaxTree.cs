namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий синтаксическое дерево. Содержит в себе
    список зафиксированных ошибок Diagnostics, само выражение Root
    и токен конца файла
    */
    public sealed class SyntaxTree{
        public SyntaxTree(DiagnosticBag diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken){
            Diagnostics = diagnostics;
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public DiagnosticBag Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        //Создает парсер и возвращает построенное дерево
        public static SyntaxTree Parse(string text){
            Parser parser = new Parser(text);
            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text){
            Lexer lexer = new Lexer(text);
            while (true){
                SyntaxToken token = lexer.Lex();
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break;

                yield return token;
            }
        }
    }
}