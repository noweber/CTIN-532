using TMPro;
using UnityEngine;

public class ResourceCountUiController : MonoBehaviour
{
    public TextMeshProUGUI CountText;

    public int ResourceCount { get; private set; }

    public void SetResourceCount(int count)
    {
        ResourceCount = count;
        if(CountText != null)
        {
            CountText.text = count.ToString();
        }
    }

    void Start()
    {
        SetResourceCount(ResourceCount);
    }

}
