using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTile : Tile
{
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
        p.success = true;
        return 0;
    }
}
