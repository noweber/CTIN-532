using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    [SerializeField]
    private Camera attachedCamera;

    [SerializeField]
    private MapGenerator mapGenerator;

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
        if (mapGenerator != null)
        {
            return mapGenerator.MapWidth * mapGenerator.TileSize / 2.0f;
        }
        return MapGenerator.Instance.MapWidth * MapGenerator.Instance.TileSize / 2.0f;
    }
}
