using System;
using Game.Domain;

namespace Game.Infrastructure
{
    public static class DrinkMasterConverter
    {
        public static DrinkData ToDomain(DrinkMasterRecord raw)
        {
            var drinkId = Enum.Parse<DrinkId>(raw.drinkId);

            var tags = Array.ConvertAll(
                raw.tags,
                t => Enum.Parse<LiquorTag>(t));

            var ingredients = Array.ConvertAll(
                raw.ingredients,
                i => Enum.Parse<IngredientId>(i));

            return new DrinkData(
                drinkId,
                raw.displayName,
                tags,
                ingredients,
                raw.unlockWeek);
        }
    }
}