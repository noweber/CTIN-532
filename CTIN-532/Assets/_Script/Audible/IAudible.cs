namespace Assets._Script.Audible
{
    public interface IAudible
    {
        void PlaySoundEffect(float? minimumPitch = null, float? maximumPitch = null, float? volume = null);

        void SetPitch(float pitch);

        void SetVolume(float pitch);

        void SetRandomizedPitch(float minimumPitch, float maximumPitch);
    }
}
