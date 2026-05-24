using System.Collections.Generic;

namespace Game.Domain
{
    public sealed class IngredientMasterTable
    {
        private readonly Dictionary<IngredientId, IngredientData> _ingredients;

        public IngredientMasterTable(Dictionary<IngredientId, IngredientData> ingredients)
        {
            _ingredients = ingredients;
        }

        public IngredientData Get(IngredientId id)
        {
            return _ingredients[id];
        }

        public bool TryGet(IngredientId id, out IngredientData ingredient)
        {
            return _ingredients.TryGetValue(id, out ingredient);
        }

        public IReadOnlyDictionary<IngredientId, IngredientData> All => _ingredients;
    }
}