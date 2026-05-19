#nullable enable

using Game.Kernel;
using UnityEngine;

namespace Game.Infrastructure
{
    public sealed class UnityRandomImpl : IRandom
    {
        public int Range(int minInclusive, int maxExclusive)
            => Random.Range(minInclusive, maxExclusive);

        public float Range(float minInclusive, float maxInclusive)
            => Random.Range(minInclusive, maxInclusive);

        public float Value01 => Random.value;
    }
}