using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MinimapHQIcon : MonoBehaviour
{
    public Sprite[] sprite_list;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(MinimapManager.Instance.HQ_scale, 
            MinimapManager.Instance.HQ_scale, MinimapManager.Instance.HQ_scale);
        transform.position= new Vector3(transform.position.x, MinimapManager.Instance.HQ_height,
            transform.position.z);
    }

    public void setOwner(MapNodeController.Player Owner)
    {
        if (sprite_list != null)
        {
            GetComponent<SpriteRenderer>().sprite = sprite_list[(int)Owner];
        }
    }

    public void highlighted(bool isTrue)
    {
        if(isTrue)
        {
            GetComponent<SpriteRenderer>().material.color = Color.yellow;
        }
        else
        {
            GetComponent<SpriteRenderer>().material.color = Color.white;
        }
    }
}
