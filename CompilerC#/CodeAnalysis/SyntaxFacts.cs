namespace CompilerCSharp.CodeAnalysis
{
    internal static class SyntaxFacts{
        //Получает приоритет оператора
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
    }
}