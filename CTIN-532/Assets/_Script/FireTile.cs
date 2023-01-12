using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTile : Tile
{
    public AudioClip clip;
    public AudioClip deadClip;

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
        StartCoroutine(dead());
        p.death++;
        return 2;
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
}
