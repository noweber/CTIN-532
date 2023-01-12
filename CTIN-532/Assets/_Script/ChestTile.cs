using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestTile : Tile
{
    bool isOpen = false;

    public AudioClip openClip;
    public AudioClip coinClip;
    public AudioClip usedClip;

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        renderer = GetComponent<MeshRenderer>();
        audio = GetComponent<AudioSource>();
        waitTime = openClip.length + coinClip.length;
        player.loadingCount++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override int entered(Player p)
    {
        if(!isOpen)
        {
            StartCoroutine(open());
            isOpen= true;
            p.hasTreature = true;
            waitTime = 1;
        }
        return 0;
    }

    IEnumerator open()
    {
        audio.clip = openClip;
        audio.Play();
        yield return new WaitForSeconds(openClip.length);
        audio.Stop();
        audio.clip = coinClip;
        audio.Play();
        yield return new WaitForSeconds(coinClip.length);
        audio.Stop();
        audio.clip = usedClip;
        yield return null;
    }
}
