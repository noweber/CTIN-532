using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTile : Tile
{
    public SecretGateTile tile;

    public AudioClip switchClip;
    public AudioClip usedClip;

    private bool isUsed = false;
    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        renderer = GetComponent<MeshRenderer>();
        audio = GetComponent<AudioSource>();
        waitTime = switchClip.length;
        player.loadingCount++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override int entered(Player p)
    {
        if(!isUsed)
        {
            StartCoroutine(trigger());
            tile.changeSound();
            isUsed= true;
            tile.isTriggered= true;
        }
        return 0;
    }

    IEnumerator trigger()
    {
        audio.clip = switchClip;
        audio.Play();
        yield return new WaitForSeconds(switchClip.length);
        audio.Stop();
        audio.clip = usedClip;
        waitTime = 1;
        yield return null;
    }
}
