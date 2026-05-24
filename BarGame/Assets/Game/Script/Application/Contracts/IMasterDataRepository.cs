using System.Collections.Generic;

namespace Game.Application.Contracts
{
    public interface IMasterDataRepository
    {
        List<T> Load<T>(string filePath);
    }
}