using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : Tile
{
    private bool hasBriage = false;

    public AudioClip clip;
    public AudioClip deadClip;
    public AudioClip makeBriageClip;
    public AudioClip safeClip;
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        renderer = GetComponent<MeshRenderer>();
        audio = GetComponent<AudioSource>();
        waitTime = deadClip.length;
        player.loadingCount++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override int entered(Player p)
    {
        if (hasBriage)
        {
            waitTime = 1;
            return 0;
        }

        if (p.woodCount < 1)
        {
            StartCoroutine(dead());
            p.death++;
            return 2;
        }
        else
        {
            StartCoroutine(briage());
            p.woodCount--;
            waitTime = makeBriageClip.length;
            hasBriage = true;
            return 0;
        }
    }

    IEnumerator dead()
    {
        audio.clip = deadClip;
        audio.Play();
        yield return new WaitForSeconds(deadClip.length);
        audio.Stop();
        audio.clip = clip;
        yield return null;
    }

    IEnumerator briage()
    {
        audio.clip = makeBriageClip;
        audio.Play();
        yield return new WaitForSeconds(makeBriageClip.length);
        audio.Stop();
        audio.clip = safeClip;
        waitTime = 1;
        yield return null;
    }
}
