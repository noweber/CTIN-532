using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private ParticleSystem particle;
    private MeshRenderer renderer;
    private bool isActive = false;

    public Material material1;
    public Material material2;
    // Start is called before the first frame update
    void Start()
    {
        particle= GetComponent<ParticleSystem>();
        renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive && !particle.isPlaying)
        {
            renderer.material = material2;
            particle.Play();
        }
        else if(!isActive && particle.isPlaying)
        {
            renderer.material = material1;
            particle.Stop();
        }
    }

    private void OnMouseDown()
    {
        isActive = !isActive;
    }
}
