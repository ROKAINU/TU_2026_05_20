using System.Collections.Generic;
using Game.Application.Contracts;

namespace Game.Infrastructure
{
    public sealed class MasterDataRepository : IMasterDataRepository
    {
        public List<T> Load<T>(string filePath)
            => MasterDataLoader.Load<T>(filePath);
    }
}