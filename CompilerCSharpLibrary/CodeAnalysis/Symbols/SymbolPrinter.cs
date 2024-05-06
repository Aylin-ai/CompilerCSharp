using System;
using System.IO;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.IO;

namespace CompilerCSharpLibrary.CodeAnalysis.Symbols
{
    public static class SymbolPrinter{
        public static void WriteTo(this Symbol symbol, TextWriter writer){
            switch (symbol.Kind){
                case SymbolKind.Type:
                    WriteTypeTo((TypeSymbol)symbol, writer);
                    break;
                case SymbolKind.Function:
                    WriteFunctionTo((FunctionSymbol)symbol, writer);
                    break;
                case SymbolKind.Parameter:
                    WriteParameterTo((ParameterSymbol)symbol, writer);
                    break;
                case SymbolKind.GlobalVariable:
                    WriteGlobalVariableTo((GlobalVariableSymbol)symbol, writer);
                    break;
                case SymbolKind.LocalVariable:
                    WriteLocalVariableTo((LocalVariableSymbol)symbol, writer);
                    break;
                default:
                    throw new Exception($"Unexpected symbol: {symbol.Kind}");
            }
        }

        private static void WriteTypeTo(TypeSymbol symbol, TextWriter writer)
        {
            writer.WriteIdentifier(symbol.Name);
        }

        private static void WriteFunctionTo(FunctionSymbol symbol, TextWriter writer)
        {
            writer.WriteKeyword("function ");
            writer.WriteIdentifier(symbol.Name);
            writer.WritePunctuation(SyntaxKind.OpenParenthesisToken);
            for (int i = 0; i < symbol.Parameters.Count; i++)
            {
                if (i > 0)
                    writer.WritePunctuation(SyntaxKind.CommaToken);

                symbol.Parameters[i].WriteTo(writer);
            }
            writer.WritePunctuation(SyntaxKind.CloseParenthesisToken);
            
            if (symbol.Type != TypeSymbol.Void)
            {
                writer.WritePunctuation(SyntaxKind.ColonToken);
                writer.WriteSpace();
                symbol.Type.WriteTo(writer);
            }
        }

        private static void WriteParameterTo(ParameterSymbol symbol, TextWriter writer)
        {
            writer.WriteIdentifier(symbol.Name);
            writer.WritePunctuation(": ");
            symbol.Type.WriteTo(writer);
        }

        private static void WriteGlobalVariableTo(GlobalVariableSymbol symbol, TextWriter writer)
        {
            writer.WriteKeyword(symbol.IsReadOnly ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword);
            writer.WriteSpace();
            writer.WriteIdentifier(symbol.Name);
            writer.WritePunctuation(SyntaxKind.ColonToken);
            writer.WriteSpace();
            symbol.Type.WriteTo(writer);
        }

        private static void WriteLocalVariableTo(LocalVariableSymbol symbol, TextWriter writer)
        {
            writer.WriteKeyword(symbol.IsReadOnly ? SyntaxKind.LetKeyword : SyntaxKind.VarKeyword);
            writer.WriteSpace();
            writer.WriteIdentifier(symbol.Name);
            writer.WritePunctuation(SyntaxKind.ColonToken);
            writer.WriteSpace();
            symbol.Type.WriteTo(writer);
        }
    }
}