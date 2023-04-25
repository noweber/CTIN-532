using UnityEngine;

public class DeactivateAfterSeconds : MonoBehaviour
{
    public float secondsToDeactivate = 5f;

    private void Start()
    {
        Invoke("DeactivateGameObject", secondsToDeactivate);
    }

    private void DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }
}
