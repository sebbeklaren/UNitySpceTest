using UnityEngine;

public class MovementPlaneController : MonoBehaviour
{
    public Transform cameraFocusPoint; // Assign the camera focus point in the Inspector

    void LateUpdate()
    {
        transform.position = cameraFocusPoint.position;
    }
}