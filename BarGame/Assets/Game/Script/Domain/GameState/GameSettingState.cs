namespace Game.Domain
{
    public readonly struct GameSettingState
    {
        public AudioVolume BGMVolume { get;} // 0.0f ~ 1.0f
        public AudioVolume SEVolume { get;} // 0.0f ~ 1.0f


        public GameSettingState(AudioVolume bgmVolume, AudioVolume seVolume)
        {
            BGMVolume = bgmVolume;
            SEVolume = seVolume;
        }

        public GameSettingState WithBGMVolume(AudioVolume value) => new(value, SEVolume);
        public GameSettingState WithSEVolume(AudioVolume value) => new(BGMVolume, value);
        
        public static readonly AudioVolume DefaultBGMVolume = new AudioVolume(0.5f);
        public static readonly AudioVolume DefaultSEVolume = new AudioVolume(0.5f);
        public static readonly GameSettingState Default = new(DefaultBGMVolume, DefaultSEVolume);
    }

    public readonly struct AudioVolume
    {
        public float Value { get; }

        public AudioVolume(float value)
        {
            Value = System.Math.Clamp(value, 0f, 1f);
        }
    }
}