
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
        //Получает тип ключевого слова
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

        public static IEnumerable<SyntaxKind> GetBinaryOperators(){
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds){
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }
        
        public static IEnumerable<SyntaxKind> GetUnaryOperators(){
            var kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (var kind in kinds){
                if (GetUnaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }

        public static string GetText(SyntaxKind kind){
            switch (kind){
                case SyntaxKind.PlusToken:
                    return "+";
                case SyntaxKind.MinusToken:
                    return "-";
                case SyntaxKind.StarToken:
                    return "*";
                case SyntaxKind.SlashToken:
                    return "/";
                case SyntaxKind.BangToken:
                    return "!";
                case SyntaxKind.EqualsToken:
                    return "=";
                case SyntaxKind.AmpersandAmpersandToken:
                    return "&&";
                case SyntaxKind.PipePipeToken:
                    return "||";
                case SyntaxKind.EqualsEqualsToken:
                    return "==";
                case SyntaxKind.NotEqualsToken:
                    return "!=";
                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.CloseParenthesisToken:
                    return ")";
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.TrueKeyword:
                    return "true";
                default:
                    return null;
            }
        }
    }
}