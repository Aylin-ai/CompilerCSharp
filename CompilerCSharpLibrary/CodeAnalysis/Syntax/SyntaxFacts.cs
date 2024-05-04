using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;

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
                case SyntaxKind.GreaterToken:
                case SyntaxKind.GreaterOrEqualsToken:
                case SyntaxKind.LessToken:
                case SyntaxKind.LessOrEqualsToken:
                    return 3;

                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AmpersandAmpersandToken:
                    return 2;

                case SyntaxKind.PipeToken:
                case SyntaxKind.HatToken:
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
                case SyntaxKind.TildeToken:
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
                case "var":
                    return SyntaxKind.VarKeyword;
                case "let":
                    return SyntaxKind.LetKeyword;
                case "if":
                    return SyntaxKind.IfKeyword;
                case "else":
                    return SyntaxKind.ElseKeyword;
                case "while":
                    return SyntaxKind.WhileKeyword;
                case "for":
                    return SyntaxKind.ForKeyword;
                case "to":
                    return SyntaxKind.ToKeyword;
                case "do":
                    return SyntaxKind.DoKeyword;
                case "function":
                    return SyntaxKind.FunctionKeyword;
                case "break":
                    return SyntaxKind.BreakKeyword;
                case "continue":
                    return SyntaxKind.ContinueKeyword;
                case "return":
                    return SyntaxKind.ReturnKeyword;
                
                default:
                    return SyntaxKind.IdentifierToken;
            }
        }

        public static IEnumerable<SyntaxKind> GetBinaryOperators(){
            SyntaxKind[]? kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (SyntaxKind kind in kinds){
                if (GetBinaryOperatorPrecedence(kind) > 0)
                    yield return kind;
            }
        }
        
        public static IEnumerable<SyntaxKind> GetUnaryOperators(){
            SyntaxKind[]? kinds = (SyntaxKind[]) Enum.GetValues(typeof(SyntaxKind));
            foreach (SyntaxKind kind in kinds){
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

                case SyntaxKind.GreaterToken:
                    return ">";
                case SyntaxKind.GreaterOrEqualsToken:
                    return ">=";
                case SyntaxKind.LessToken:
                    return "<";
                case SyntaxKind.LessOrEqualsToken:
                    return "<=";

                case SyntaxKind.OpenParenthesisToken:
                    return "(";
                case SyntaxKind.CloseParenthesisToken:
                    return ")";

                case SyntaxKind.OpenBraceToken:
                    return "{";
                case SyntaxKind.CloseBraceToken:
                    return "}";

                case SyntaxKind.CommaToken:
                    return ",";
                case SyntaxKind.ColonToken:
                    return ":";
                case SyntaxKind.SemiColonToken:
                    return ";";

                case SyntaxKind.AmpersandToken:
                    return "&";
                case SyntaxKind.PipeToken:
                    return "|";
                case SyntaxKind.TildeToken:
                    return "~";
                case SyntaxKind.HatToken:
                    return "^";
                    
                case SyntaxKind.FalseKeyword:
                    return "false";
                case SyntaxKind.TrueKeyword:
                    return "true";
                case SyntaxKind.VarKeyword:
                    return "var";
                case SyntaxKind.LetKeyword:
                    return "let";
                case SyntaxKind.ReturnKeyword:
                    return "return";
                case SyntaxKind.IfKeyword:
                    return "if";
                case SyntaxKind.ElseKeyword:
                    return "else";
                case SyntaxKind.WhileKeyword:
                    return "while";
                case SyntaxKind.ForKeyword:
                    return "for";
                case SyntaxKind.ToKeyword:
                    return "to";
                case SyntaxKind.DoKeyword:
                    return "do";
                case SyntaxKind.FunctionKeyword:
                    return "function";
                case SyntaxKind.BreakKeyword:
                    return "break";
                case SyntaxKind.ContinueKeyword:
                    return "continue";
                default:
                    return null;
            }
        }
    }
}