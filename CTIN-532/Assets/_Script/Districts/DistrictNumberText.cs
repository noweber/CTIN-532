using UnityEngine;
using TMPro;
using Assets._Script;

public class DistrictNumberText : MonoBehaviour
{
    public TextMeshProUGUI DistrictNumber;

    private void FixedUpdate()
    {
        if (DistrictNumber != null)
        {
             DistrictNumber.text = DependencyService.Instance.DistrictController().DistrictNumber.ToString();
             //DistrictNumber.text = GameObject.FindObjectOfType<GameManager>().DistrictNumber.ToString();
        }
    }
}
