using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseoverMove : MonoBehaviour
{
    public Vector3 MovementMagnitude;

    public Vector3 MovementSpeed;

    private bool isMoving;

    private Vector3 currentOffset;

    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        currentOffset = new Vector3();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isMoving)
        {
            if(MovementMagnitude == null || MovementSpeed == null)
            {
                Debug.LogError("Movement information is null.");
                return;
            }

            Vector3 offsetDelta = MovementSpeed * Time.fixedDeltaTime;
            currentOffset += offsetDelta;
            transform.position += offsetDelta;
            if(currentOffset.x >= MovementMagnitude.x && currentOffset.y >= MovementMagnitude.y && currentOffset.z >= MovementMagnitude.z)
            {
                isMoving = false;
            }
        }
    }

    public void OnMouseOver()
    {
        isMoving = true;
    }

    public void OnMouseExit()
    {
        isMoving = false;
        transform.position -= currentOffset;
        currentOffset = new Vector3();
    }
}
