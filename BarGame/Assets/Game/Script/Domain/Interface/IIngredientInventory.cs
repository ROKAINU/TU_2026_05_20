using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public interface IIngredientInventory
    {
        bool HasIngredient(IngredientId id);

        bool ConsumeMultiple(IReadOnlyList<IngredientId> ingredients);

        int GetStock(IngredientId id);
    }
}