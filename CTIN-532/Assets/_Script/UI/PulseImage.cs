using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PulseImage : MonoBehaviour
{

    public float pulseSpeed = 1.0f; // Speed of the pulse animation
    public Color endColor = Color.gray; // Color to pulse to
    public Color startColor = Color.white; // Color to pulse from

    private Image image; // Reference to the Image component

    void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(Pulse());
    }

    IEnumerator Pulse()
    {
        while (true)
        {
            image.color = endColor;
            yield return new WaitForSeconds(1.0f / pulseSpeed);
            image.color = startColor;
            yield return new WaitForSeconds(1.0f / pulseSpeed);
        }
    }
}
