using UnityEngine;

public class MoveUp : MonoBehaviour
{
    public float Speed = 1.0f;

    void Update()
    {
        transform.Translate(new Vector3(0, Speed * Time.deltaTime, 0));
    }
}
