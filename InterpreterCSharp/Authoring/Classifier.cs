using System;
using System.Collections.Generic;
using CompilerCSharpLibrary.CodeAnalysis.Syntax;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using CompilerCSharpLibrary.CodeAnalysis.Text;

namespace CompilerCSharpLibrary.CodeAnalysis.Authoring
{
    public static class Classifier
    {
        public static List<ClassifiedSpan> Classify(SyntaxTree syntaxTree, TextSpan span)
        {
            var result = new List<ClassifiedSpan>();
            ClassifyNode(syntaxTree.Root, span, result);
            return result;
        }

        private static void ClassifyNode(SyntaxNode node, TextSpan span, List<ClassifiedSpan> result)
        {
            // HACK: node should never be null, but that's tracked by #141
            if (node == null || !node.FullSpan.OverlapsWith(span))
                return;

            if (node is SyntaxToken token)
                ClassifyToken(token, span, result);

            foreach (var child in node.GetChildren())
                ClassifyNode(child, span, result);
        }

        private static void ClassifyToken(SyntaxToken token, TextSpan span, List<ClassifiedSpan> result)
        {
            foreach (var leadingTrivia in token.LeadingTrivia)
                ClassifyTrivia(leadingTrivia, span, result);

            AddClassification(token.Kind, token.Span, span, result);

            foreach (var trailingTrivia in token.TrailingTrivia)
                ClassifyTrivia(trailingTrivia, span, result);
        }

        private static void ClassifyTrivia(SyntaxTrivia trivia, TextSpan span, List<ClassifiedSpan> result)
        {
            AddClassification(trivia.Kind, trivia.Span, span, result);
        }

        private static void AddClassification(SyntaxKind elementKind, TextSpan elementSpan, TextSpan span, List<ClassifiedSpan> result)
        {
            if (!elementSpan.OverlapsWith(span))
                return;

            var adjustedStart = Math.Max(elementSpan.Start, span.Start);
            var adjustedEnd = Math.Min(elementSpan.End, span.End);
            var adjustedSpan = TextSpan.FromBounds(adjustedStart, adjustedEnd);
            var classification = GetClassification(elementKind);

            var classifiedSpan = new ClassifiedSpan(adjustedSpan, classification);
            result.Add(classifiedSpan);
        }

        private static Classification GetClassification(SyntaxKind kind)
        {
            var isKeyword = kind.IsKeyword();
            var isIdentifier = kind == SyntaxKind.IdentifierToken;
            var isNumber = kind == SyntaxKind.NumberToken;
            var isString = kind == SyntaxKind.StringToken;
            var isComment = kind.IsComment();

            if (isKeyword)
                return Classification.Keyword;
            else if (isIdentifier)
                return Classification.Identifier;
            else if (isNumber)
                return Classification.Number;
            else if (isString)
                return Classification.String;
            else if (isComment)
                return Classification.Comment;
            else
                return Classification.Text;
        }
    }
}