using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain
{
    /// <summary>
    /// 調合レシピ（DrinkId + 必要素材）
    /// </summary>
    public sealed class DrinkRecipe
    {
        public DrinkId                     DrinkId     { get; }
        public IReadOnlyList<IngredientId> Ingredients { get; }
        public IngredientSet               IngredientSet { get; }

        public DrinkRecipe(DrinkId drinkId, IReadOnlyList<IngredientId> ingredients)
        {
            DrinkId       = drinkId;
            Ingredients   = ingredients;
            IngredientSet = IngredientSet.From(ingredients);
        }
    }

    /// <summary>
    /// 調合の逆引き・調合可能チェック
    /// </summary>
    public sealed class RecipeMasterTable
    {
        private readonly Dictionary<DrinkId, DrinkRecipe>       _byDrinkId;
        private readonly Dictionary<IngredientSet, DrinkRecipe> _byIngredientSet;

        public RecipeMasterTable(IEnumerable<DrinkRecipe> recipes)
        {
            _byDrinkId       = new Dictionary<DrinkId, DrinkRecipe>();
            _byIngredientSet = new Dictionary<IngredientSet, DrinkRecipe>();

            foreach (var recipe in recipes)
            {
                _byDrinkId[recipe.DrinkId]           = recipe;
                _byIngredientSet[recipe.IngredientSet] = recipe;
            }
        }

        /// <summary>DrinkId からレシピ取得</summary>
        public bool TryGet(DrinkId id, out DrinkRecipe recipe)
            => _byDrinkId.TryGetValue(id, out recipe);

        /// <summary>素材の組み合わせからレシピを逆引き（順序不問）</summary>
        public bool TryFindByIngredients(
            IReadOnlyList<IngredientId> ingredients,
            out DrinkRecipe recipe)
        {
            var key = IngredientSet.From(ingredients);
            return _byIngredientSet.TryGetValue(key, out recipe);
        }

        /// <summary>在庫から作れるレシピを全件取得</summary>
        public IEnumerable<DrinkRecipe> GetCraftable(IIngredientInventory inventory)
        {
            foreach (var recipe in _byDrinkId.Values)
            {
                if (recipe.Ingredients.All(id => inventory.HasIngredient(id)))
                    yield return recipe;
            }
        }
    }
}