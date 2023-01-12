using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : Tile
{
    public AudioClip clip;
    public AudioClip enterClip;

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

    public override int entered(Player p)
    {
        StartCoroutine(bump());
        return 1;
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
