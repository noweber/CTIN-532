using DG.Tweening;
using TMPro;
using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    public Material Icon_Material;

    public Material[] Buttons_Material;

    public TextMeshProUGUI[] Text;

    private void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(Icon_Material.DOFloat(1.9f,"_Progress",3));

        float time = 1.7f; 
        for(int i = 0; i<Buttons_Material.Length; i++)
        {
            sequence.Insert(time + i * 0.5f , Buttons_Material[i].DOFloat(1.8f, "_Progress", 1));
            sequence.Insert(time + i * 0.5f, Text[i].DOFade(1,1f));
        }


    }

    private void OnDestroy()
    {
        Icon_Material.SetFloat("_Progress", -1);
        for (int i = 0; i < Buttons_Material.Length; i++)
        {
            Buttons_Material[i].SetFloat("_Progress", -0.1f);
        }
    }

}
