namespace Game.Domain
{
    public readonly struct SettingSaveData
    {
        //SettingData
        public float BGMVolume { get; }
        public float SEVolume { get; }
        
        public SettingSaveData(
            float bgmVolume,
            float seVolume)
        {
            BGMVolume = bgmVolume;
            SEVolume = seVolume;
        }

        public static SettingSaveData Default() => new SettingSaveData(
            bgmVolume: 0.5f,
            seVolume: 0.5f
        );

        public SettingSaveData With(
            float? bgmVolume = null,
            float? seVolume = null)
        {
            return new SettingSaveData(
            bgmVolume: bgmVolume ?? BGMVolume,
            seVolume: seVolume ?? SEVolume
            );
        }
    }
}