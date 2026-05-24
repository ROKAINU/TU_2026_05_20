using System.Collections.Generic;

namespace Game.Domain
{
    /// <summary>
    /// ドリンク評価結果
    /// </summary>
    public enum DrinkMatchResult
    {
        Perfect, // 完全一致
        // 酒Idの一致はTagParfectを含む完全一致の一種である
        TagPerfect, // 客タグ ⊆ 酒タグ
        Partial, // 部分一致
        None     // 一致なし
    }

    /// <summary>
    /// 酒の評価ロジック
    /// </summary>
    public static class DrinkEvaluate
    {
        /// <summary>
        /// 客の要求と提供ドリンクを評価する
        /// </summary>
        /// <param name="requestedDrinkData">客の要求ドリンク</param>
        /// <param name="drink">提供ドリンク</param>
        /// <returns>評価結果</returns>
        public static DrinkMatchResult Match(
            DrinkData requestedDrinkData,
            DrinkData drink)
        {
            // DrinkId完全一致
            if (requestedDrinkData != null &&
                requestedDrinkData.DrinkId == drink.DrinkId)
            {
                return DrinkMatchResult.Perfect;
            }

            // タグ評価
            if (requestedDrinkData == null ||
                requestedDrinkData.Tags == null ||
                requestedDrinkData.Tags.Count == 0 ||
                drink.Tags == null ||
                drink.Tags.Count == 0)
            {
                return DrinkMatchResult.None;
            }

            var drinkSet = new HashSet<LiquorTag>(drink.Tags);
            int matchCount = 0;

            foreach (var tag in requestedDrinkData.Tags)
            {
                if (drinkSet.Contains(tag))
                    matchCount++;
            }

            if (matchCount == 0)
                return DrinkMatchResult.None;

            return matchCount == requestedDrinkData.Tags.Count
                ? DrinkMatchResult.TagPerfect
                : DrinkMatchResult.Partial;
        }
    }
}