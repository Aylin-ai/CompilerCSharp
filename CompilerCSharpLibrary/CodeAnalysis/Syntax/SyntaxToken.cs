using CompilerCSharpLibrary.CodeAnalysis.Text;
using CompilerCSharpLibrary.CodeAnalysis.Syntax.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace CompilerCSharpLibrary.CodeAnalysis.Syntax
{
    /*
    Класс, представляющий токен и все его атрибуты: тип, позиция в тексте,
    лексема и значение, если есть. У данного токена нет дочерних токенов
    */
    public sealed class SyntaxToken : SyntaxNode
    {

        #region Поля класса

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object? Value { get; }
        public override TextSpan Span => new TextSpan(Position, Text.Length);
        public override TextSpan FullSpan
        {
            get
            {
                var start = LeadingTrivia.Count == 0
                                ? Span.Start
                                : LeadingTrivia.First().Span.Start;
                var end = TrailingTrivia.Count == 0
                                ? Span.End
                                : TrailingTrivia.Last().Span.End;
                return TextSpan.FromBounds(start, end);
            }
        }

        /// <summary>
        /// A token is missing if it was inserted by the parser and doesn't appear in source.
        /// </summary>
        public bool IsMissing { get; }

        public List<SyntaxTrivia> LeadingTrivia { get; }
        public List<SyntaxTrivia> TrailingTrivia { get; }

        #endregion

        #region Конструкторы класса

        internal SyntaxToken(SyntaxTree syntaxTree,
                             SyntaxKind kind,
                             int position,
                             string? text,
                             object? value,
                             List<SyntaxTrivia> leadingTrivia,
                             List<SyntaxTrivia> trailingTrivia)
            : base(syntaxTree)
        {
            Kind = kind;
            Position = position;
            Text = text ?? string.Empty;
            IsMissing = text == null;
            Value = value;
            LeadingTrivia = leadingTrivia;
            TrailingTrivia = trailingTrivia;
        }

        #endregion

        #region Методы класса 

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Array.Empty<SyntaxNode>();
        }

        #endregion

    }
}