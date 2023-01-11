using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Camera mainCamera;

    // Use this for initialization
    void Start()
    {
        this.mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Face the main camera:
        if (this.mainCamera != null)
        {
            transform.LookAt(this.mainCamera.transform);
        }
    }
}
