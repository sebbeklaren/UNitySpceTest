using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SelectionManager : MonoBehaviour
{
    public List<GameObject> selectedShips = new List<GameObject>();
    public LayerMask shipLayer;
    public LayerMask movementPlaneLayer;

    private Vector2 startMousePosition;
    private Vector2 endMousePosition;
    private bool isDragging = false;

    private bool isSettingVerticalPosition = false;
    private Vector3 initialTargetPosition;
    private float verticalOffset = 0f;

    public GameObject movementIndicator;
    public Transform directionArrow;  // drag the child from Inspector

    public LineRenderer pathLineRenderer; // Drag the LineRenderer here
    private bool isDrawingPath = false;

    private bool isMovementMode = false;    // True after pressing M
    private bool isPlacingTarget = false;   // True while left-click is held to set a target
    private Vector3 moveCommandOrigin;      // Where circle starts (e.g. ships' average pos)
   


    void Update()
    {
        HandleSelectionInput();
        //HandleMoveInput();
        if(isDrawingPath)
        {
            UpdatePathLine();
        }
        if (Input.GetKeyDown(KeyCode.M) && selectedShips.Count > 0)
        {
            // Either turn it on or off
            isMovementMode = !isMovementMode;

            if (isMovementMode)
            {
                // Calculate centroid of selected ships
                Vector3 avgPos = Vector3.zero;
                foreach (GameObject ship in selectedShips)
                {
                    avgPos += ship.transform.position;
                }
                moveCommandOrigin = avgPos / selectedShips.Count;

                // Reset vertical offset
                verticalOffset = 0f;

                // Show the circle
                movementIndicator.SetActive(true);

                // Place it at the ships' centroid
                movementIndicator.transform.position = moveCommandOrigin;

                // Start small scale
                movementIndicator.transform.localScale = Vector3.zero;
                Debug.Log("Move mode ON");
            }
            else
            {
                // Hide the circle
                movementIndicator.SetActive(false);
                Debug.Log("Move mode OFF");
            }
        }

        // If we are in Move Mode, handle the mouse input for placing target
        if (isMovementMode)
        {
            HandleMoveModeInput();
            UpdateCircleScale();
        }
    }
    void HandleMoveModeInput()
    {
        // 1) Mouse button down => Start "placing target"
        if (!isPlacingTarget && Input.GetMouseButtonDown(0))
        {
            isPlacingTarget = true;
        }

        // 2) While left-click is held => update vertical offset
        if (isPlacingTarget && Input.GetMouseButton(0))
        {
            float mouseYDelta = Input.GetAxis("Mouse Y");
            verticalOffset += mouseYDelta * 0.2f;

            // Move the circle up/down
            Vector3 newPos = moveCommandOrigin + new Vector3(0, verticalOffset, 0);
            movementIndicator.transform.position = newPos;
        }

        // 3) Mouse button up => finalize the move
        if (isPlacingTarget && Input.GetMouseButtonUp(0))
        {
            isPlacingTarget = false;
            Vector3 finalPosition = moveCommandOrigin + new Vector3(0, verticalOffset, 0);

            // Issue move command
            MoveUnitsInFormation(finalPosition);

            Debug.Log("Issued move command to: " + finalPosition);

            // Optionally hide or fade the circle after confirming
            // movementIndicator.SetActive(false);
            // isMovementMode = false;
            // But you might choose to keep isMovementMode active for repeated moves
        }
    }
    void UpdateCircleScale()
    {
        // 1) Raycast to your movement plane
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, movementPlaneLayer))
            return;

        // 2) Center of your ring in world space
        Vector3 ringCenter = moveCommandOrigin + new Vector3(0, verticalOffset, 0);

        // 3) Direction from ring center to mouse
        Vector3 direction = hit.point - ringCenter;

        // If you want the ring always horizontal, remove any Y component
        // direction.y = 0f;

        // 4) Distance and scaled ring size
        float distance = direction.magnitude;
        float scaleVal = distance * 0.2f;   // 0.2f is just an example scale
        scaleVal = Mathf.Clamp(scaleVal, 0f, 50f);

        // 5) Apply ring scale 
        //    (assuming the ring is modeled so scale=1 is radius=1)
        movementIndicator.transform.position = ringCenter; // Make sure it's centered
        movementIndicator.transform.localScale = new Vector3(scaleVal, 1f, scaleVal);

        // 6) Position and rotate the arrow at the edge
        if (directionArrow != null)
        {
            Vector3 dirNormalized = direction.normalized;

            // The arrow sits at the circle's perimeter:
            Vector3 arrowPos = ringCenter + dirNormalized * scaleVal * 5.2f;
            // Explanation: if your ring mesh at scale=1 is radius=1, 
            // then at localScale.x=‘scaleVal’, the radius is ‘scaleVal/2’.
            // But you might do ‘scaleVal’ directly if your ring mesh is half-size, etc.

            directionArrow.position = arrowPos;

            // Face outward along 'direction'
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dirNormalized, Vector3.up);
                directionArrow.rotation = lookRot;
            }
            else
            {
                // If mouse is basically at center, no rotation
                directionArrow.rotation = Quaternion.identity;
            }
        }
    }


    void HandleSelectionInput()
    {
        // Skip selection logic if Left Shift is held (to avoid deselecting units)
        if (Input.GetKey(KeyCode.LeftShift))
            return;
        // Start drag selection
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Input.mousePosition;
            isDragging = true;
        }

        // Update end position while dragging
        if (isDragging)
        {
            endMousePosition = Input.mousePosition;
        }

        // End drag selection
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            SelectUnitsWithinDrag();
        }
    }

    void SelectUnitsWithinDrag()
    {
        selectedShips.Clear();

        Rect selectionRect = Utils.GetScreenRect(startMousePosition, endMousePosition);

        foreach (GameObject ship in GameObject.FindGameObjectsWithTag("CombatShip"))
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(ship.transform.position);

            // Check that the ship is in front of the camera
            if (screenPos.z < 0)
                continue;

            screenPos.y = Screen.height - screenPos.y; // Flip Y coordinate for GUI space

            if (selectionRect.Contains(screenPos, true))
            {
                selectedShips.Add(ship);
                Debug.Log("Selected ship: " + ship.name);

                // Optional: visually highlight selected ships
               // HighlightShip(ship, true);
            }
            else
            {
                // Optional: clear highlight for unselected ships
               // HighlightShip(ship, false);
            }
        }
    }

    void HandleMoveInput()
    {
        
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift) && selectedShips.Count > 0 && !isSettingVerticalPosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("MovemenPlane")))
            {
                initialTargetPosition = hit.point;
                verticalOffset = 0f;
                isSettingVerticalPosition = true;

                // STOP any running fade coroutines
                StopAllCoroutines();

                // Reset indicator visibility & color
                movementIndicator.SetActive(true);
                Material indicatorMaterial = movementIndicator.GetComponent<Renderer>().material;
                indicatorMaterial.color = new Color(1f, 1f, 1f, 1f); // Fully visible
                movementIndicator.transform.position = initialTargetPosition;


                // Start drawing the path
                ResetLineRendererAlpha();
                isDrawingPath = true;
                pathLineRenderer.positionCount = 0; // Ensure it's reset before adding points
                UpdatePathLine();
            }
        }

        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift) && isSettingVerticalPosition)
        {
            float mouseYDelta = Input.GetAxis("Mouse Y");
            verticalOffset += mouseYDelta * 0.2f;
            Debug.Log("Vertical offset: " + verticalOffset);
        }

        if (Input.GetMouseButtonUp(0) && isSettingVerticalPosition)
        {
            Vector3 finalPosition = initialTargetPosition + new Vector3(0, verticalOffset, 0);
            MoveUnitsInFormation(finalPosition);
           

            isSettingVerticalPosition = false;
            verticalOffset = 0f;
            // Hide indicator after units start moving
            movementIndicator.SetActive(false);
            // Start fade check AFTER movement begins
            StartCoroutine(CheckIfUnitsArrived(finalPosition));
        }
    }

    void UpdatePathLine()
    {
        pathLineRenderer.startWidth = 0.1f;
        pathLineRenderer.endWidth = 0.1f;
        if (!isDrawingPath || selectedShips.Count == 0) return;

        pathLineRenderer.positionCount = selectedShips.Count * 2;

        for (int i = 0; i < selectedShips.Count; i++)
        {
            GameObject ship = selectedShips[i];

            // Start position (ship position)
            pathLineRenderer.SetPosition(i * 2, ship.transform.position);

            // End position (target destination)
            pathLineRenderer.SetPosition(i * 2 + 1, movementIndicator.transform.position);
        }
    }

    void MoveUnitsInFormation(Vector3 destinationCenter)
    {
        int numUnits = selectedShips.Count;
        int unitsPerRow = Mathf.CeilToInt(Mathf.Sqrt(numUnits));
        float spacing = 2f;

        Vector3 right = Camera.main.transform.right;
        Vector3 forward = Vector3.Cross(right, Vector3.up);

        int currentRow = 0;
        int currentColumn = 0;

        foreach (GameObject ship in selectedShips)
        {
            Vector3 offset = (right * (currentColumn - unitsPerRow / 2f)
                            + forward * (currentRow - unitsPerRow / 2f)) * spacing;

            Vector3 targetPosition = destinationCenter + offset;
            ship.GetComponent<ShipMovementController>().MoveTo(targetPosition);

            currentColumn++;
            if (currentColumn >= unitsPerRow)
            {
                currentColumn = 0;
                currentRow++;
            }
        }
    }

    void HighlightShip(GameObject ship, bool selected)
    {
        if (selected)
        {
            ship.layer = LayerMask.NameToLayer("Selected");
        }
        else
        {
            ship.layer = LayerMask.NameToLayer("CombatShip"); // Or whatever layer it was on before
        }
    }

    void OnGUI()
    {
        if (isDragging)
        {
            Rect rect = Utils.GetScreenRect(startMousePosition, endMousePosition);
            Utils.DrawScreenRect(rect, new Color(0f, 0.8f, 1f, 0.25f)); // Transparent box
            Utils.DrawScreenRectBorder(rect, 2, Color.cyan);            // Border
        }
    }
    private IEnumerator CheckIfUnitsArrived(Vector3 destination)
    {
        float fadeDuration = 5f; // Time for full fade out
        float fadeStartDistance = 5f; // When to start fading
        float elapsedTime = 0f;
        bool allArrived = false;

        Material indicatorMaterial = movementIndicator.GetComponent<Renderer>().material;
        Color initialIndicatorColor = indicatorMaterial.color;

        while (!allArrived)
        {
            allArrived = true;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject ship in selectedShips)
            {
                float distance = Vector3.Distance(ship.transform.position, destination);
                if (distance > 0.5f)
                {
                    allArrived = false;
                }

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }

            // Start fading when closest unit is within fade range
            if (closestDistance < fadeStartDistance)
            {
                // Calculate elapsed time based on distance
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

                // Apply fade effect to indicator
                indicatorMaterial.color = new Color(initialIndicatorColor.r, initialIndicatorColor.g, initialIndicatorColor.b, alpha);

                // Apply fade effect to line
                UpdateLineRendererAlpha(alpha);
            }

            UpdatePathLine(); // Keep updating the line positions
            yield return null; // Wait for the next frame
        }

        // Fully hide when all units arrive
        pathLineRenderer.positionCount = 0;
        movementIndicator.SetActive(false);
        isDrawingPath = false;
        pathLineRenderer.enabled = false; // Fully hide line
        ResetLineRendererAlpha(); // Ensure line is reset for next movement

        Debug.Log("All units arrived. Path and indicator faded out.");
    }

    void UpdateLineRendererAlpha(float alpha)
    {
        Gradient newGradient = new Gradient();
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(alpha, 0f);
        alphaKeys[1] = new GradientAlphaKey(alpha, 1f);

        GradientColorKey[] colorKeys = pathLineRenderer.colorGradient.colorKeys;

        newGradient.SetKeys(colorKeys, alphaKeys);
        pathLineRenderer.colorGradient = newGradient;
    }

    void ResetLineRendererAlpha()
    {
        UpdateLineRendererAlpha(1f); // Reset to fully visible
    }

}