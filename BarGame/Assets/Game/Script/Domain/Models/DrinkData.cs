using System.Collections.Generic;

namespace Game.Domain
{
    [System.Serializable]
    public sealed class DrinkData
    {
        public DrinkId DrinkId { get; }
        public string DisplayName { get; }

        public IReadOnlyList<LiquorTag> Tags { get; }
        public IReadOnlyList<IngredientId> Ingredients { get; }

        public int UnlockWeek { get; }

        public DrinkData(
            DrinkId drinkId,
            string displayName,
            IReadOnlyList<LiquorTag> tags,
            IReadOnlyList<IngredientId> ingredients,
            int unlockWeek)
        {
            DrinkId = drinkId;
            DisplayName = displayName;
            Tags = tags;
            Ingredients = ingredients;
            UnlockWeek = unlockWeek;
        }
    }
}