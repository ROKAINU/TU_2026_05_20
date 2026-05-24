using System;
using System.Collections.Generic;
using Game.Domain;

namespace Game.Application.Contracts
{
    public interface IIngredientInventory
    {
        bool HasIngredient(IngredientId id);

        bool ConsumeMultiple(IReadOnlyList<IngredientId> ingredients);

        int GetStock(IngredientId id);
    }
}