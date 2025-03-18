using UnityEngine;

public class PathLineRendererController : MonoBehaviour
{
    public LineRenderer pathLineRenderer;
    public SelectionManager selectionManager;

    void Update()
    {
        UpdatePathLines();
    }

    void UpdatePathLines()
    {
        if (selectionManager.selectedShips.Count == 0) return;

        pathLineRenderer.positionCount = selectionManager.selectedShips.Count * 2;

        for (int i = 0; i < selectionManager.selectedShips.Count; i++)
        {
            GameObject ship = selectionManager.selectedShips[i];
            ShipMovementController movement = ship.GetComponent<ShipMovementController>();
            if (movement == null) continue;

            Vector3 shipPosition = ship.transform.position;
            Vector3 destinationPosition = movement.GetTargetPosition();

            pathLineRenderer.SetPosition(i * 2, shipPosition);
            pathLineRenderer.SetPosition(i * 2 + 1, destinationPosition);
        }
    }
}