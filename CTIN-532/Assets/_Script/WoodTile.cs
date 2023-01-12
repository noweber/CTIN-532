using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodTile : Tile
{
    public AudioClip clip;
    public AudioClip collectClip;

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        renderer = GetComponent<MeshRenderer>();
        audio = GetComponent<AudioSource>();
        waitTime = collectClip.length;
        player.loadingCount++;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override int entered(Player p)
    {
        p.woodCount++;
        StartCoroutine(collect());
        return 0;
    }

    IEnumerator collect()
    {
        audio.clip = collectClip;
        audio.Play();
        yield return new WaitForSeconds(collectClip.length);
        audio.Stop();
        audio.clip = clip;
        yield return null;
    }
}
