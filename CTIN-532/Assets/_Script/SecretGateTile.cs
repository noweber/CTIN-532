using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretGateTile : Tile
{
    [HideInInspector]
    public bool isTriggered = false;

    public AudioClip clip;
    public AudioClip enterClip;
    public AudioClip doorClip;

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        renderer = GetComponent<MeshRenderer>();
        audio = GetComponent<AudioSource>();
        player.loadingCount++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeSound()
    {
        audio.clip = doorClip;
    }

    public override int entered(Player p)
    {
        if(!isTriggered)
        {
            StartCoroutine(bump());
            return 1;
        }
        else
        {
            return 0;
        }
    }

    IEnumerator bump()
    {
        audio.clip = enterClip;
        audio.Play();
        yield return new WaitForSeconds(enterClip.length);
        audio.Stop();
        audio.clip = clip;
        yield return null;
    }
}
