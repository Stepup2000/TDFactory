using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Makes sure the objects faces the active camera.
/// and always faces the camera.
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    [SerializeField, Tooltip("If true, the text popup will not take the original rotation into consideration.")]
    private bool overrideOriginalRotation = true;

    private Camera targetCamera;
    private Quaternion originalRotation;

    /// <summary>
    /// Called when the script is enabled. Subscribes to necessary events.
    /// </summary>
    private void OnEnable()
    {
        SetupObject();
        if (CameraController.Instance != null) CameraController.Instance.OnCameraChange += ChangeTargetCamera;
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from events to avoid potential memory leaks.
    /// </summary>
    private void OnDisable()
    {
        if (CameraController.Instance != null) CameraController.Instance.OnCameraChange -= ChangeTargetCamera;
    }

    /// <summary>
    /// Updates the reference to the current camera from the CameraController.
    /// </summary>
    private void ChangeTargetCamera()
    {
        if (CameraController.Instance != null)
        {
            targetCamera = CameraController.Instance.GetCurrentCamera();
        }
    }

    /// <summary>
    /// Initializes the health bar, setting the proper camera and health values.
    /// </summary>
    private void SetupObject()
    {
        ChangeTargetCamera();
        originalRotation = transform.rotation;
        TurnToCamera();
    }

    /// <summary>
    /// Rotates the health bar to always face the current camera.
    /// </summary>
    private void TurnToCamera()
    {
        if (targetCamera != null)
        {
            if (overrideOriginalRotation == false) transform.rotation = targetCamera.transform.rotation * originalRotation;
            else transform.rotation = targetCamera.transform.rotation;
        }
    }

    /// <summary>
    /// Updates the health bar's rotation to face the camera every frame.
    /// </summary>
    private void Update()
    {
        TurnToCamera();
    }
}
