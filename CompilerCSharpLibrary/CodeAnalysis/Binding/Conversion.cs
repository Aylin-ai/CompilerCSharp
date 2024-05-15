using CompilerCSharpLibrary.CodeAnalysis.Symbols;

namespace CompilerCSharpLibrary.CodeAnalysis.Binding
{
    public sealed class Conversion
    {
        //Implicity - неявное преобразование
        //Explicity - явное

        public static readonly Conversion None = new Conversion(exist: false, isIdentity: false, isImplicit: false);
        //Identity - типы равны
        public static readonly Conversion Identity = new Conversion(exist: true, isIdentity: true, isImplicit: true);
        //При преобразовании информация теряется (float -> int)
        public static readonly Conversion Implicit = new Conversion(exist: true, isIdentity: false, isImplicit: true);
        //При преобразовании информация не теряется (int -> float)
        public static readonly Conversion Explicit = new Conversion(exist: true, isIdentity: false, isImplicit: false);
        private Conversion(bool exist,
                           bool isIdentity,
                           bool isImplicit)
        {
            Exist = exist;
            IsIdentity = isIdentity;
            IsImplicit = isImplicit;
        }

        public bool Exist { get; }
        public bool IsIdentity { get; }
        public bool IsImplicit { get; }
        public bool IsExplicit => Exist && !IsImplicit;

        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
                return Identity;

            if (from != TypeSymbol.Void && to == TypeSymbol.Any)
            {
                return Implicit;
            }

            if (from == TypeSymbol.Any && to != TypeSymbol.Void)
            {
                return Explicit;
            }

            if (from == TypeSymbol.Int || from == TypeSymbol.Bool)
            {
                if (to == TypeSymbol.String)
                    return Explicit;
            }

            if (from == TypeSymbol.String)
            {
                if (to == TypeSymbol.Int || to == TypeSymbol.Bool)
                    return Explicit;
            }

            return None;
        }

    }
}