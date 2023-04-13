using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseCursor : MonoBehaviour
{
    public Texture2D cursor;

    Vector2 hotSpot = new Vector2(0, 0);
    CursorMode cursorMode = CursorMode.Auto;

    public float distance;
    public Camera camera;

    private void Start()
    {
        Cursor.SetCursor(cursor, hotSpot, cursorMode);
    }

    private void Update()
    {
        Vector3 p = Input.mousePosition;
        p.z = distance;
        transform.position = camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
