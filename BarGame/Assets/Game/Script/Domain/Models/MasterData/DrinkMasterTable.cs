using System.Collections.Generic;

namespace Game.Domain
{
    public sealed class DrinkMasterTable
    {
        private readonly Dictionary<DrinkId, DrinkData> _drinks;

        public DrinkMasterTable(Dictionary<DrinkId, DrinkData> drinks)
        {
            _drinks = drinks;
        }

        public DrinkData Get(DrinkId id)
        {
            return _drinks[id];
        }

        public bool TryGet(DrinkId id, out DrinkData drink)
        {
            return _drinks.TryGetValue(id, out drink);
        }

        public IReadOnlyDictionary<DrinkId, DrinkData> All => _drinks;
    }
}