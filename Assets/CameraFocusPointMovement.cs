using UnityEngine;

public class CameraFocusPointMovement : MonoBehaviour
{
    public float panSpeed = 10f;
    public SelectionManager selectionManager; // Assign in Inspector

    private float yaw, pitch; // Camera rotation variables
    private Vector3 targetPosition; // Target position for smooth movement
    private Vector3 velocity = Vector3.zero; // For SmoothDamp
    public float smoothTime = 0.3f; // Time to reach the target (adjust for speed)
    private bool isMovingToTarget = false; // Track if the camera is moving to a target

    void Update()
    {
        Movement();
        HandleFocusInput();

        // Smoothly move the camera to the target position if needed
        if (isMovingToTarget)
        {
            SmoothMoveToTarget();
        }
    }

    void Movement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) move -= Vector3.forward;
        if (Input.GetKey(KeyCode.A)) move -= Vector3.right;
        if (Input.GetKey(KeyCode.D)) move += Vector3.right;
        if (Input.GetKey(KeyCode.Q)) move += Vector3.down;
        if (Input.GetKey(KeyCode.E)) move += Vector3.up;

        if (move != Vector3.zero)
        {
            transform.Translate(move.normalized * panSpeed * Time.deltaTime, Space.Self);
        }
    }

    void HandleFocusInput()
    {
        if (Input.GetKeyDown(KeyCode.F) && selectionManager.selectedShips.Count > 0)
        {
            UpdateTargetPosition();
            isMovingToTarget = true; // Start smooth movement
            Debug.Log("Camera focusing on selected ships");
        }
    }

    void UpdateTargetPosition()
    {
        if (selectionManager.selectedShips.Count == 0) return;

        // Calculate the average position of selected ships
        Vector3 total = Vector3.zero;
        foreach (GameObject ship in selectionManager.selectedShips)
        {
            total += ship.transform.position;
        }
        targetPosition = total / selectionManager.selectedShips.Count;

        // Reset camera angles when new ships are selected
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Debug.Log("Target Position Set: " + targetPosition);
    }

    void SmoothMoveToTarget()
    {
        if (targetPosition != Vector3.zero)
        {
            // Smoothly move the camera to the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // Check if the camera is close to the target
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            Debug.Log("Distance to Target: " + distanceToTarget);

            if (distanceToTarget < 0.1f)
            {
                isMovingToTarget = false; // Stop smooth movement
                Debug.Log("Camera reached target position");
            }
        }
    }
}