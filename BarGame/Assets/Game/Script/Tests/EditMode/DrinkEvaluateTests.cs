// Tests/EditMode/DrinkEvaluateTests.cs
using System.Collections.Generic;
using NUnit.Framework;
using Game.Domain;

namespace Game.Tests.EditMode
{
    /// <summary>
    /// DrinkEvaluate の評価ロジックテスト
    /// 
    /// テスト内容
    /// Match_DrinkIdが一致すればPerfectか
    ///     同じHighball同士Perfect
    /// Match_DrinkIdが違いタグが完全一致ならTagPerfectか
    ///     HighballとWhiskeySoda（どちらもRefreshing+Light）TagPerfect
    /// Match_タグが一部一致ならPartialか
    ///     客はRefreshing+Light、提供はRefreshing+StrongPartial
    /// Match_タグが一切一致しなければNoneか
    ///     客はSweet+Calm、提供はRefreshing+LightNone
    /// Match_requestedがnullならNoneか
    ///     客データがnull
    /// NoneMatch_requestedのタグが空ならNoneか
    ///     タグなしでもDrinkIdが一致すればPerfect
    /// 最後のテストは少し注意が必要で、「タグが空でもDrinkIdが一致するならPerfect」という仕様を確認しています。タグの有無よりDrinkIdの一致が優先されることを保証しています。
    /// </summary>
    public class DrinkEvaluateTests
    {
        // ---- テスト用DrinkDataファクトリ ----

        private static DrinkData MakeDrink(DrinkId id, params LiquorTag[] tags)
            => new DrinkData(id, id.ToString(), tags, System.Array.Empty<IngredientId>(), 1);

        // ---- Perfectテスト ----

        [Test]
        public void Match_DrinkIdが一致すればPerfect()
        {
            var requested = MakeDrink(DrinkId.Highball, LiquorTag.Refreshing, LiquorTag.Light);
            var served    = MakeDrink(DrinkId.Highball, LiquorTag.Refreshing, LiquorTag.Light);

            var result = DrinkEvaluate.Match(requested, served);

            Assert.AreEqual(DrinkMatchResult.Perfect, result);
        }

        // ---- TagPerfectテスト ----

        [Test]
        public void Match_DrinkIdが違いタグが完全一致ならTagPerfect()
        {
            var requested = MakeDrink(DrinkId.Highball,  LiquorTag.Refreshing, LiquorTag.Light);
            var served    = MakeDrink(DrinkId.WhiskeySoda, LiquorTag.Refreshing, LiquorTag.Light);

            var result = DrinkEvaluate.Match(requested, served);

            Assert.AreEqual(DrinkMatchResult.TagPerfect, result);
        }

        // ---- Partialテスト ----

        [Test]
        public void Match_タグが一部一致ならPartial()
        {
            // requested: Refreshing + Light
            // served:    Refreshing + Strong  → Refreshingだけ一致
            var requested = MakeDrink(DrinkId.Highball,  LiquorTag.Refreshing, LiquorTag.Light);
            var served    = MakeDrink(DrinkId.GinSoda,   LiquorTag.Refreshing, LiquorTag.Strong);

            var result = DrinkEvaluate.Match(requested, served);

            Assert.AreEqual(DrinkMatchResult.Partial, result);
        }

        // ---- Noneテスト ----

        [Test]
        public void Match_タグが一切一致しなければNone()
        {
            var requested = MakeDrink(DrinkId.KaluaMilk, LiquorTag.Sweet, LiquorTag.Calm);
            var served    = MakeDrink(DrinkId.Highball,  LiquorTag.Refreshing, LiquorTag.Light);

            var result = DrinkEvaluate.Match(requested, served);

            Assert.AreEqual(DrinkMatchResult.None, result);
        }

        [Test]
        public void Match_requestedがnullならNone()
        {
            var served = MakeDrink(DrinkId.Highball, LiquorTag.Refreshing, LiquorTag.Light);

            var result = DrinkEvaluate.Match(null, served);

            Assert.AreEqual(DrinkMatchResult.None, result);
        }

        [Test]
        public void Match_requestedのタグが空ならNone()
        {
            var requested = MakeDrink(DrinkId.Highball); // タグなし
            var served    = MakeDrink(DrinkId.Highball, LiquorTag.Refreshing);

            // DrinkIdは一致するのでPerfectになることを確認
            var result = DrinkEvaluate.Match(requested, served);
            Assert.AreEqual(DrinkMatchResult.Perfect, result);
        }
    }
}