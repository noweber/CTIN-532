using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    public AudioSource SFXPlayer;

    private float min_pitch = -0.9f;
    private float max_pitch = 1.1f;

    // TODO: Remove these. These were added to get prototype 4 working ASAP.
    public AudioSource SpawnSound;

    public AudioSource FightSound;

    public AudioSource SelectSound;

    public AudioSource GainNodeSound;

    public AudioSource LoseNodeSound;

    // Used for UI
    public void PlaySFX(AudioClip audioClip, float volume)
    {
        SFXPlayer.PlayOneShot(audioClip, volume);
    }

    // Used for repeat-play SFX
    public void PlayRandomSFX(AudioClip audioClip,float volume)
    {
        SFXPlayer.pitch = Random.Range(min_pitch,max_pitch);
        PlaySFX(audioClip, volume);
    }

}
