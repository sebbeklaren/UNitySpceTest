using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("Ship Movement Settings")]
    [Tooltip("Maximum ship speed")]
    public float shipSpeed = 5f;

    [Tooltip("How fast the ship accelerates to max speed")]
    public float shipAcceleration = 2f;

    [Tooltip("Rotation speed when turning towards target")]
    public float rotationSpeed = 2f;

    [Header("Indicator Settings")]
    [Tooltip("How long the movement indicator takes to fade out")]
    public float indicatorFadeTime = 5f;

    [Tooltip("Distance from target before indicator starts fading")]
    public float indicatorFadeStartDistance = 5f;

    [Header("Path Line Settings")]
    [Tooltip("Width of the path line")]
    public float pathLineWidth = 0.05f;

    [Tooltip("Time before the path line fully fades")]
    public float pathFadeDuration = 5f;

    [Header("Formation Settings")]
    [Tooltip("Spacing between ships in formation")]
    public float formationSpacing = 2f;

    [Header("Camera Zoom Settings")]
    [Tooltip("Zoom speed when using the scroll wheel")]
    public float zoomSpeed = 5f;

    [Tooltip("Minimum zoom distance (how close the camera can get)")]
    public float minZoom = 5f;

    [Tooltip("Maximum zoom distance (how far the camera can go)")]
    public float maxZoom = 30f;
}
