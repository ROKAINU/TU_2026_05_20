#nullable enable
using System.Collections.Generic;

namespace Game.Domain
{
    // 顧客の注文
    public sealed class CustomerOrder
    {
        // 指名注文（なければ null）
        public DrinkId? RequestedDrinkId { get; }

        // 求める特徴
        public IReadOnlyList<LiquorTag> DesiredTags { get; }

        // 追加の注文条件を表すプロパティ
        
        public CustomerOrder(DrinkId? requestedDrinkId, IReadOnlyList<LiquorTag> desiredTags)
        {
            RequestedDrinkId = requestedDrinkId;
            DesiredTags = desiredTags;
        }
    }
}