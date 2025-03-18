using UnityEngine;

public static class Utils
{
    // Returns a rect drawn between two points
    public static Rect GetScreenRect(Vector2 screenPosition1, Vector2 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        return Rect.MinMaxRect(
            Mathf.Min(screenPosition1.x, screenPosition2.x),
            Mathf.Min(screenPosition1.y, screenPosition2.y),
            Mathf.Max(screenPosition1.x, screenPosition2.x),
            Mathf.Max(screenPosition1.y, screenPosition2.y));
    }

    // Draw rectangle with GUI
    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    // Draw rectangle border
    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color); // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color); // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color); // Left
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color); // Right
    }
}
