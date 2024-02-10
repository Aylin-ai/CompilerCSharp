
namespace CompilerCSharp.CodeAnalysis.Syntax
{
    internal static class SyntaxFacts{
        //Получает приоритет бинарного оператора
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind){
            switch (kind){
                case SyntaxKind.SlashToken:
                case SyntaxKind.StarToken:
                    return 2;
                    
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;

                default:
                    return 0;
            }
        }
        //Получает приоритет унарного оператора
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind){
            switch (kind){
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 3;

                default:
                    return 0;
            }
        }

        public static SyntaxKind GetKeywordKind(string text)
        {
            switch (text){
                case "true":
                    return SyntaxKind.TrueKeyword;
                case "false":
                    return SyntaxKind.FalseKeyword;
                
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }
    }
}