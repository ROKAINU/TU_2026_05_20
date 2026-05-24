namespace Game.Domain
{
    // 顧客の状態
    public sealed class CustomerState
    {
        // 顧客の表示名
        public string DisplayName { get; }
        
        // 注文
        public CustomerOrder Order { get; }

        // その他、顧客の状態を表すプロパティ
    }
}