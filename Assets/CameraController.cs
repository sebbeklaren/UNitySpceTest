using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraFocusPoint; // Assign the focus point in the Inspector
   // public float panSpeed = 10f;
    public float rotationSpeed = 5f;
    public Vector2 verticalClamp = new Vector2(-80f, 80f);
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 50f;

    private float yaw, pitch;
    private float currentZoom;

    void Start()
    {
        currentZoom = Vector3.Distance(transform.position, cameraFocusPoint.position);
    }

    void Update()
    {
        HandleCameraRotation();
        HandleCameraZoom();
    }   

    void HandleCameraRotation()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, verticalClamp.x, verticalClamp.y);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 position = cameraFocusPoint.position - (rotation * Vector3.forward * currentZoom);

        transform.position = position;
        transform.rotation = rotation;
    }

    void HandleCameraZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentZoom -= scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
    }
}