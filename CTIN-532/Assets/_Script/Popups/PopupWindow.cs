using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindow : MonoBehaviour
{
    public bool canNext = false;
    public bool canPrev = false;

    public GameObject nextButton;
    public GameObject prevButton;

    virtual public void next()
    {
        canNext = true;
    }

    virtual public void prev()
    {
        canPrev = true;
    }

    public void setActive(bool active)
    {
        canNext = false;
        canPrev= false;
        gameObject.SetActive(active);
    }

}
