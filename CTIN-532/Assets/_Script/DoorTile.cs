using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class DoorTile : Tile
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
        return base.entered(p);
    }
}
