namespace Game.Domain
{
    // 材料データ（マスターから読み込み）
    [System.Serializable]
    public class IngredientData
    {
        public IngredientId   IngredientId { get; set; }
        public string         DisplayName  { get; set; }
        public IngredientType Type         { get; set; }
        public LiquorTag[]    Tags         { get; set; }
        public int            UnlockWeek   { get; set; }
    }
}