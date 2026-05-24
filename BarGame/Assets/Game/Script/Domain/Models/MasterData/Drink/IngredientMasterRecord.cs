namespace Game.Domain
{
    // JSON読み込み用（IngredientMaster.json）
    [System.Serializable]
    public class IngredientMasterRecord
    {
        public const string FilePath = "IngredientMaster";
        
        public string   ingredientId;   // "Whiskey"
        public string   displayName;    // "ウイスキー（ベース）"
        public string   type;           // "Base", "Liqueur", "Mixer"
        public string[] tags;           // ["Strong", "Bitter"]
        public int      unlockWeek;     // 1
    }
}