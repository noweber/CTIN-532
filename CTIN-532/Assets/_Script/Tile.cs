using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    protected ParticleSystem particle;
    protected MeshRenderer renderer;
    protected AudioSource audio;

    [HideInInspector]
    public float waitTime = 1;

    public bool isActive = false;
    public bool isLast = false;

    public Material material1;
    public Material material2;

    public Tile up;
    public Tile down;
    public Tile left;
    public Tile right;

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        particle= GetComponent<ParticleSystem>();
        renderer = GetComponent<MeshRenderer>();
        audio = GetComponent<AudioSource>();
        player.loadingCount++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*private void OnMouseDown()
    {
        isActive = !isActive;
        if (isActive && !particle.isPlaying)
        {
            select();
        }
        else if (!isActive && particle.isPlaying)
        {
            deSelect();
        }
    }*/

    public void select()
    {
        renderer.material = material2;
        particle.Play();
    }

    public void deSelect()
    {
        renderer.material = material1;
        particle.Stop();
    }

    public void highLightOff()
    {
        deSelect();
        if (left != null){left.deSelect();    }
        if (up != null) { up.deSelect(); }
        if (right != null) { right.deSelect(); }
        if (down != null) { down.deSelect(); }
    }

    public void play()
    {
        audio.Play();
    }
    public void stop() { 
        audio.Stop();
    }

    public float getLength()
    {
        return audio.clip.length;
    }

    // 0 - success; 1 - not moving; 2 - dead
    virtual public int entered(Player p)
    {
        return 0;
    }
}
