// Tests/EditMode/RecipeMasterTableTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using Game.Domain;
using Game.Infrastructure;

namespace Game.Tests.EditMode
{
    /// <summary>
    /// RecipeMasterTable の調合逆引き・調合可能チェックテスト
    /// 
    /// テスト内容
    /// 1.逆引き
    /// TryFindByIngredients_正しい組み合わせでレシピを引けるか
    ///     [Whiskey, Soda] → Highball が返るか
    /// TryFindByIngredients_順序が違っても同じレシピを引けるか
    ///     [Soda, Whiskey] でも Highball が返るか
    /// TryFindByIngredients_存在しない組み合わせはfalseを返すか
    ///     [Whiskey, Milk] のような未登録の組み合わせが false を返すか
    /// TryFindByIngredients_3素材のレシピを引けるか
    ///     [Campari, Vermouth, Gin]（順序バラバラ）でも Negroni が返るか
    /// 2.調合
    /// GetCraftable_在庫がある素材のレシピだけ返るか
    ///     WhiskeyとSodaだけ在庫があるとき、Highballだけ返るか
    /// GetCraftable_在庫が空なら何も返らない
    ///     在庫ゼロのとき空リストが返るか
    /// GetCraftable_素材が揃えば複数返る
    ///     HighballとGinTonicの素材が両方あるとき2件返るか
    /// </summary>
    public class RecipeMasterTableTests
    {
        private RecipeMasterTable  _recipeTable;
        private IngredientInventory _inventory;

        [SetUp]
        public void SetUp()
        {
            // ハイボール: Whiskey + Soda
            // ジントニック: Gin + TonicWater
            // ネグローニ: Gin + Campari + Vermouth（3素材）
            var recipes = new List<DrinkRecipe>
            {
                new DrinkRecipe(DrinkId.Highball,
                    new[] { IngredientId.Whiskey, IngredientId.Soda }),
                new DrinkRecipe(DrinkId.GinTonic,
                    new[] { IngredientId.Gin, IngredientId.TonicWater }),
                new DrinkRecipe(DrinkId.Negroni,
                    new[] { IngredientId.Gin, IngredientId.Campari, IngredientId.Vermouth }),
            };

            _recipeTable = new RecipeMasterTable(recipes);
            _inventory   = new IngredientInventory();
        }

        // ---- 逆引きテスト ----

        [Test]
        public void TryFindByIngredients_正しい組み合わせでレシピを引ける()
        {
            var found = _recipeTable.TryFindByIngredients(
                new[] { IngredientId.Whiskey, IngredientId.Soda },
                out var recipe);

            Assert.IsTrue(found);
            Assert.AreEqual(DrinkId.Highball, recipe.DrinkId);
        }

        [Test]
        public void TryFindByIngredients_順序が違っても同じレシピを引ける()
        {
            // Soda → Whiskey の順で渡す
            var found = _recipeTable.TryFindByIngredients(
                new[] { IngredientId.Soda, IngredientId.Whiskey },
                out var recipe);

            Assert.IsTrue(found);
            Assert.AreEqual(DrinkId.Highball, recipe.DrinkId);
        }

        [Test]
        public void TryFindByIngredients_存在しない組み合わせはfalseを返す()
        {
            var found = _recipeTable.TryFindByIngredients(
                new[] { IngredientId.Whiskey, IngredientId.Milk },
                out _);

            Assert.IsFalse(found);
        }

        [Test]
        public void TryFindByIngredients_3素材のレシピを引ける()
        {
            var found = _recipeTable.TryFindByIngredients(
                new[] { IngredientId.Campari, IngredientId.Vermouth, IngredientId.Gin },
                out var recipe);

            Assert.IsTrue(found);
            Assert.AreEqual(DrinkId.Negroni, recipe.DrinkId);
        }

        // ---- 調合可能チェックテスト ----

        [Test]
        public void GetCraftable_在庫がある素材のレシピだけ返る()
        {
            _inventory.Add(IngredientId.Whiskey, 1);
            _inventory.Add(IngredientId.Soda, 1);
            // Gin / TonicWater / Campari / Vermouth は未追加

            var craftable = new List<DrinkRecipe>(
                _recipeTable.GetCraftable(_inventory));

            Assert.AreEqual(1, craftable.Count);
            Assert.AreEqual(DrinkId.Highball, craftable[0].DrinkId);
        }

        [Test]
        public void GetCraftable_在庫が空なら何も返らない()
        {
            var craftable = new List<DrinkRecipe>(
                _recipeTable.GetCraftable(_inventory));

            Assert.AreEqual(0, craftable.Count);
        }

        [Test]
        public void GetCraftable_素材が揃えば複数返る()
        {
            _inventory.Add(IngredientId.Whiskey,   1);
            _inventory.Add(IngredientId.Soda,      1);
            _inventory.Add(IngredientId.Gin,       1);
            _inventory.Add(IngredientId.TonicWater,1);

            var craftable = new List<DrinkRecipe>(
                _recipeTable.GetCraftable(_inventory));

            Assert.AreEqual(2, craftable.Count);
        }
    }
}