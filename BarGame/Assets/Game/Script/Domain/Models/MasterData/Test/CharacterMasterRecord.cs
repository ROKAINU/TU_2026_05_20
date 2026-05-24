namespace Game.Domain
{
    // Excelの列名と一致させる（シリアライズはJsonUtility使用）   
    [System.Serializable]
    public class CharacterMasterRecord
    {
        public const string FilePath = "CharacterMaster";
        public int    id;
        public string name;
        public int    level;
        public int    hp;
        public float  attack;
        public string[] skills;
        public bool   active;
    }
}