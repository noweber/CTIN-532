using Assets._Script;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    private CameraAction cameraAction;
    private InputAction move;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    public float smoothTime = 0.3f;
    public float move_speed = 1.0f;
    public float zoomRate = 10f;
    public float min_Zoom_heigh = 20f;
    public float max_Zoom_heigh = 350f;

    private void Awake()
    {
        cameraAction = new CameraAction();
        targetPosition = transform.position;
    }

    private void OnEnable()
    {
        move = cameraAction.Player.Move;
        //cameraAction.Player.Zoom.performed += zoomCamera;
        cameraAction.Player.Enable();
    }

    private void OnDisable()
    {
        cameraAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (DependencyService.Instance.DistrictFsm().CurrentState != DistrictState.Play)
        {
            return;
        }

        GetKeyboardMovement();
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void GetKeyboardMovement()
    {
        Vector3 inputValue = move.ReadValue<Vector2>();

        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f)
        {
            targetPosition += new Vector3(inputValue.x, 0, inputValue.y) * move_speed;
        }
    }

    private void ZoomCamera(InputAction.CallbackContext inputeValue)
    {
        float value = -inputeValue.ReadValue<Vector2>().y;

        if (Mathf.Abs(value) > 0.1f)
        {
            float targetHeight = targetPosition.y + value * zoomRate;

            if (targetHeight < min_Zoom_heigh)
            {
                targetHeight = min_Zoom_heigh;
            }
            else if (targetHeight > max_Zoom_heigh)
            {
                targetHeight = max_Zoom_heigh;
            }

            targetPosition.y = targetHeight;
        }
    }

    public void SetFocus(Vector3 focusedPos)
    {
        targetPosition = new Vector3(focusedPos.x, focusedPos.y + 3, focusedPos.z - 6);
        transform.position = new Vector3(focusedPos.x, focusedPos.y + 3, focusedPos.z - 6);
    }
}
