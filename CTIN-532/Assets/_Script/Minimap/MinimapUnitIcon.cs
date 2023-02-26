using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapUnitIcon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(MinimapManager.Instance.unit_scale, 
            MinimapManager.Instance.unit_scale, MinimapManager.Instance.unit_scale);
        transform.position= new Vector3(transform.position.x, MinimapManager.Instance.unit_height,
            transform.position.z);
    }
}
