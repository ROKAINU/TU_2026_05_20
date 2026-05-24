namespace Game.Domain
{
    [System.Serializable]
    public class DrinkMasterRecord
    {
        public const string FilePath = "DrinkMaster";
        public string   drinkId;        // "Highball"
        public string   displayName;    // "ハイボール"
        public string[] tags;           // ["Refreshing", "Light"]
        public string[] ingredients;    // ["Whiskey", "Soda"]
        public int      unlockWeek;     // 1
    }
}