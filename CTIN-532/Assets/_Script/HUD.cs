using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public TextMeshProUGUI resouce_text;
    // Start is called before the first frame update
    public void setResoucesText(int tiles, float revenue, float cost)
    {
        resouce_text.GetComponent<TextMeshProUGUI>().text = 
            "Tile: " + tiles + " Revenue: " + revenue + " Cost: " + cost;
    }
}
