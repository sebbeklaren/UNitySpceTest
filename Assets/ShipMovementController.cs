using UnityEngine;

public class ShipMovementController : MonoBehaviour
{
    public float shipSpeed = 10f;
    public float rotationSpeed = 5f;
    public float acceleration = 2f;

    private Vector3 targetPosition;
    private bool shouldMove = false;
    private float currentSpeed = 0f;

    private bool isVerticalDragging = false;
    private Vector3 initialVerticalPoint;

    void Update()
    {
        if (shouldMove)
        {
            RotateTowardsTarget();
            MoveForward();
        }
    }

    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
        shouldMove = true;
    }

    void RotateTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void MoveForward()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, shipSpeed, acceleration * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            shouldMove = false;
            currentSpeed = 0f;
        }
    }


    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }
}