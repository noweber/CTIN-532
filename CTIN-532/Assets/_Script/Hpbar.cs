using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hpbar : MonoBehaviour
{
    public GameObject bar;
    private Vector3 targetScale;

    private void Start()
    {
        targetScale= bar.transform.localScale;
    }

    private void FixedUpdate()
    {
        bar.transform.localScale = Vector3.Lerp(bar.transform.localScale, targetScale, Time.deltaTime);
    }

    public void updateHpBar(float Hptotal, float HpCurrent)
    {
        targetScale.x = HpCurrent/Hptotal;
        Debug.Log("targetScale" + targetScale);
    }
}
