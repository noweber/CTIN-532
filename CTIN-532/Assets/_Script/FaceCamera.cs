using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField]
    private float secondsDelayBetweenCameraFace = 0.5f;

    /// <summary>
    /// The seconds since the last time this object faced the camera are stored to reduce the amount of facing calls.
    /// </summary>
    private float secondsSinceLastCameraFace;

    private Camera mainCamera;

    private void Awake()
    {
        secondsSinceLastCameraFace = 0;
    }

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
            secondsSinceLastCameraFace += Time.fixedDeltaTime;
            if (secondsSinceLastCameraFace > secondsDelayBetweenCameraFace)
            {
                // Set seconds to face to zero since having a perfectly accurate number of faces per interval is not the goal:
                secondsSinceLastCameraFace = 0;
                transform.LookAt(this.mainCamera.transform);
            }
        }
    }
}
