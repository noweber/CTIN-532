using UnityEngine;
using TMPro;

public class DistrictNumberText : MonoBehaviour
{
    public TextMeshProUGUI DistrictNumber;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (DistrictNumber != null && gameManager != null && !string.Equals(DistrictNumber.text, gameManager.NumberOfDistrictLevelsCleared.ToString()))
        {
            DistrictNumber.text = gameManager.NumberOfDistrictLevelsCleared.ToString();
        }
    }
}
