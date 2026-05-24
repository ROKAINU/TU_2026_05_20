using System;
using Game.Domain;

namespace Game.Infrastructure
{
    public static class IngredientMasterConverter
    {
        public static IngredientData ToDomain(IngredientMasterRecord raw)
        {
            var ingredientId = Enum.Parse<IngredientId>(raw.ingredientId);
            var type = Enum.Parse<IngredientType>(raw.type);

            var tags = Array.ConvertAll(
                raw.tags ?? System.Array.Empty<string>(),
                t => Enum.Parse<LiquorTag>(t));

            return new IngredientData
            {
                IngredientId = ingredientId,
                DisplayName = raw.displayName,
                Type = type,
                Tags = tags,
                UnlockWeek = raw.unlockWeek
            };
        }
    }
}