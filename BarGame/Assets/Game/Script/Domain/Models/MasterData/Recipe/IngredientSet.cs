using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain
{
    /// <summary>
    /// IngredientId の集合をビットフラグで表現する値型。
    /// 順序不問の組み合わせキーとして使う。
    /// </summary>
    public readonly struct IngredientSet : IEquatable<IngredientSet>
    {
        private readonly ulong _bits;

        private IngredientSet(ulong bits) => _bits = bits;

        public static IngredientSet From(IReadOnlyList<IngredientId> ingredients)
        {
            ulong bits = 0;
            foreach (var id in ingredients)
                bits |= 1UL << (int)id;
            return new IngredientSet(bits);
        }

        public bool Contains(IngredientId id)
            => (_bits & (1UL << (int)id)) != 0;

        public bool Equals(IngredientSet other) => _bits == other._bits;
        public override bool Equals(object obj) => obj is IngredientSet s && Equals(s);
        public override int GetHashCode() => _bits.GetHashCode();
        public static bool operator ==(IngredientSet a, IngredientSet b) => a._bits == b._bits;
        public static bool operator !=(IngredientSet a, IngredientSet b) => a._bits != b._bits;
    }
}