using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortOrder : MonoBehaviour
{
    public bool isMoveing = false;

    private SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(gameObject + ": " +(transform.position.z * 1000));
        //Debug.Log(gameObject + ": " + (int)(transform.position.z * 1000));
        renderer = GetComponent<SpriteRenderer>();
        renderer.sortingOrder = -(int)(transform.position.z * 1000);
        //Debug.Log(renderer.sortingOrder);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoveing)
        {
            renderer.sortingOrder = (int)(transform.position.z * 100);
        }
    }
}
