using UnityEngine;

public class DestroyOnTimer : MonoBehaviour
{
    public int SecondsUntilDestruction = 4;

    void Start()
    {
        Destroy(transform.gameObject, SecondsUntilDestruction);
    }
}
