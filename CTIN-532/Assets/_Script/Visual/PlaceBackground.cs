using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UIElements;

public class PlaceBackground : MonoBehaviour
{
    public List<GameObject> prefabs;
    private int num_of_fragments = 8;
    public float distance;

    [SerializeField]
    private List<GameObject> fragments;
    private float angle_delta;
    private float sprite_length;

    private void Start()
    {
        fragments = new List<GameObject>();
        sprite_length = prefabs[0].GetComponent<SpriteRenderer>().bounds.size.x;
        CreateBackground();
    }

    [ContextMenu("Create_Background")]
    private void CreateBackground()
    {
        int prefab_index = 0;
        float angle = 0;
        angle_delta = 360 / num_of_fragments;
        for (int i = 0; i< num_of_fragments; i++)
        {
            fragments.Add(Instantiate(prefabs[prefab_index],transform));
            fragments[i].transform.Rotate(new Vector3(0f,angle,0f));
            angle += angle_delta;
            prefab_index++;
            if(prefab_index >= prefabs.Count)
            {
                prefab_index = 0;
            }
        }
        setScale();
    }

    private void setScale()
    {
        float length = (2 * Mathf.Sqrt(2) - 2) * distance;
        float scale = length / sprite_length;
        Debug.Log("Length: " + length + "; Scale: " + scale + "; Sprite_Length: " + sprite_length);
        foreach(GameObject obj in fragments)
        {
            obj.transform.localScale = new Vector3(scale, scale, 1);
            obj.transform.localPosition = distance * - obj.transform.forward;
        }
    }

    [ContextMenu("Remove_Background")]
    private void RemoveBackground()
    {
        foreach (GameObject obj in fragments)
        {
            Destroy(obj);
        }
        fragments = new List<GameObject>();
    }
}
