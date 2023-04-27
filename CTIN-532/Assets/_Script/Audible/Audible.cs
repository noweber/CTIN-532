using UnityEngine;

namespace Assets._Script.Audible
{
    public class Audible : MonoBehaviour, IAudible
    {
        protected const float UNITY_PITCH_AMPLITUDE = 3f;

        protected const float UNITY_MINIMUM_VOLUME = 0;

        protected const float UNITY_MAXIMUM_VOLUME = 1f;

        [SerializeField]
        protected AudioSource AudioSource = null;

        protected float Pitch { get; private set; } = 1f;

        protected float Volume { get; private set; } = 1f;

        protected virtual void Start()
        {
            if (AudioSource == null)
            {
                // TODO: Test whether or not it matters which audio source is used.
                if (!TryGetComponent(out AudioSource))
                {
                    AudioSource = gameObject.AddComponent<AudioSource>();
                }
            }
        }

        public void PlaySoundEffect(float? minimumPitch = null, float? maximumPitch = null, float? volume = null)
        {
            float pitchForThisPlay = Pitch;
            if (minimumPitch != null && minimumPitch.HasValue && maximumPitch != null && maximumPitch.HasValue)
            {
                if (IsPitchRangeValidInUnity(minimumPitch.Value, maximumPitch.Value))
                {
                    pitchForThisPlay = Random.Range(minimumPitch.Value, maximumPitch.Value);
                }
            }
            float volumeForThisPlay = Volume;
            if (volume != null && volume.HasValue)
            {
                if (IsVolumeValidInUnity(volume.Value))
                {
                    volumeForThisPlay = volume.Value;
                }
            }
            if (AudioSource != null)
            {
                float previousPitch = AudioSource.pitch;
                float previousVolume = AudioSource.volume;
                AudioSource.pitch = pitchForThisPlay;
                AudioSource.volume = volumeForThisPlay;
                AudioSource.Play();
                AudioSource.pitch = previousPitch;
                AudioSource.volume = previousVolume;
            }
        }

        public void SetPitch(float pitch)
        {
            if (!IsPitchRangeValidInUnity(pitch, pitch))
            {
                Debug.LogWarning("Pitch is invalid.");
                return;
            }
            Pitch = pitch;
        }

        public void SetVolume(float volume)
        {
            if (!IsVolumeValidInUnity(volume))
            {
                Debug.LogWarning("Volume is invalid.");
                return;
            }
            Volume = volume;
        }

        public void SetRandomizedPitch(float minimumPitch, float maximumPitch)
        {
            if (!IsPitchRangeValidInUnity(minimumPitch, maximumPitch))
            {
                Debug.LogWarning("Pitch range is invalid.");
                return;
            }
            Pitch = Random.Range(minimumPitch, maximumPitch);
        }

        protected static bool IsPitchRangeValidInUnity(float minimum, float maxiumim)
        {
            if (minimum < -UNITY_PITCH_AMPLITUDE || maxiumim < -UNITY_PITCH_AMPLITUDE)
            {
                return false;
            }
            else if (minimum > UNITY_PITCH_AMPLITUDE || maxiumim > UNITY_PITCH_AMPLITUDE)
            {
                return false;
            }
            else if (minimum > maxiumim)
            {
                return false;
            }
            return true;
        }

        protected static bool IsVolumeValidInUnity(float volume)
        {
            if (volume < UNITY_MINIMUM_VOLUME || volume > UNITY_MAXIMUM_VOLUME)
            {
                return false;
            }

            return true;
        }

        protected static float GetValidPitch()
        {
            return Random.Range(-UNITY_PITCH_AMPLITUDE, UNITY_PITCH_AMPLITUDE);
        }

        protected static float GetValidVolume()
        {
            return Random.Range(UNITY_MINIMUM_VOLUME, UNITY_MAXIMUM_VOLUME);
        }
    }
}
