
namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    public static class SyntaxFacts{
        //Получает приоритет бинарного оператора
        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind){
            switch (kind){
                case SyntaxKind.SlashToken:
                case SyntaxKind.StarToken:
                    return 5;
                    
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 4;

                case SyntaxKind.NotEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                    return 3;

                case SyntaxKind.AmpersandAmpersandToken:
                    return 2;

                case SyntaxKind.PipePipeToken:
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
                case SyntaxKind.BangToken:
                    return 6;

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