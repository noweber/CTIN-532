using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Window_Transition : MonoBehaviour
{
    Sequence load;
    Sequence unload;
    public Material window_Material;
    public TextMeshProUGUI[] Text;
    // Start is called before the first frame update

    public void openWindow()
    {
        gameObject.SetActive(true);
        load = DOTween.Sequence();
        load.Append(window_Material.DOFloat(1.8f, "_Progress", 1.5f));
        for (int i = 0; i < Text.Length; i++)
        {
            load.Insert(0.5f, Text[i].DOFade(1, 0.5f));
        }
    }

    public void closeWindow()
    {
        unload = DOTween.Sequence();
        unload.Append(window_Material.DOFloat(-0.25f, "_Progress", 1.5f));
        for (int i = 0; i < Text.Length; i++)
        {
            unload.Insert(0.5f, Text[i].DOFade(0, 0.5f));
        }
        unload.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void OnDestroy()
    {
        window_Material.SetFloat("_Progress", -0.25f);
    }
}
