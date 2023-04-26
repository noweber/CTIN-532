using Assets._Script;
using UnityEngine;

public class DistrictMinimapCameraController : MonoBehaviour
{
    [SerializeField]
    private Camera attachedCamera;

    private void Awake()
    {
        if (attachedCamera == null)
        {
            attachedCamera = gameObject.GetComponent<Camera>();
        }
    }

    void FixedUpdate()
    {
        transform.position = GetCameraPosition();
        attachedCamera.orthographicSize = GetMapHalfWidth();
    }

    private Vector3 GetCameraPosition()
    {
        return new Vector3(GetMapHalfWidth(), GetMapHalfWidth(), GetMapHalfWidth()); ;
    }

    private float GetMapHalfWidth()
    {
        return DependencyService.Instance.DistrictController().DistrictSize.x * 1.0f / 2.0f;
    }
}
