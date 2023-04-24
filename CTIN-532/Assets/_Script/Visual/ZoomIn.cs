using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ZoomIn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
            }

    private void OnEnable()
    {
        transform.DOScale(new Vector3(1, 1, 1), 0.2f);
    }

    private void OnDisable()
    {
        transform.DOScale(new Vector3(0, 0, 0), 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
