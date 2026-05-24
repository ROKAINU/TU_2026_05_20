using System.Collections.Generic;
using Game.Application.Contracts;
using Game.Domain;

namespace Game.Infrastructure
{
    public sealed class IngredientInventory : IIngredientInventory
    {
        private readonly Dictionary<IngredientId, int> _stock = new();

        public void Add(IngredientId id, int quantity)
        {
            if (_stock.TryGetValue(id, out var current))
                _stock[id] = current + quantity;
            else
                _stock[id] = quantity;
        }

        public bool ConsumeMultiple(IReadOnlyList<IngredientId> ingredients)
        {
            foreach (var id in ingredients)
            {
                if (!_stock.TryGetValue(id, out var qty) || qty < 1)
                    return false;
            }

            foreach (var id in ingredients)
                _stock[id]--;

            return true;
        }

        public int GetStock(IngredientId id)
            => _stock.TryGetValue(id, out var qty) ? qty : 0;

        public bool HasIngredient(IngredientId id)
            => GetStock(id) > 0;
    }
}