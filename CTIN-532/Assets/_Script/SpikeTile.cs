using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SpikeTile : Tile
{

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        renderer = GetComponent<MeshRenderer>();
        audio = GetComponent<AudioSource>();
        waitTime = audio.clip.length;
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

    public IEnumerator dead()
    {
        audio.Play();
        yield return new WaitForSeconds(audio.clip.length);
        audio.Stop();
        yield return null;
    }
}
