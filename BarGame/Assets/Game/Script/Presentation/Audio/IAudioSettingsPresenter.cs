namespace Game.Presentation
{
    public interface IAudioSettingsPresenter
    {
        void SetVolume(VolumeType volumeType, float value);
        float GetDecibel(VolumeType volumeType);
    }
}