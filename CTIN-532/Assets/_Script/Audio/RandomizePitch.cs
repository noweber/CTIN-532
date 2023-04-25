using UnityEngine;

public sealed class RandomizePitch : MonoBehaviour
{
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        RandomizeAudioPitchAndPlay();
    }

    public void RandomizeAudioPitchAndPlay()
    {
        if (audioSource != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            if (audioSource.clip != null)
            {
                audioSource.Play();
            }
        }
    }
}
