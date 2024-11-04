using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the registration of a camera with the CameraController. This script
/// automatically registers the camera instance with the CameraController when
/// the scene is loaded.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraObject : MonoBehaviour
{
    [SerializeField] private string cameraName; // The name to associate with this camera in the CameraController.

    /// <summary>
    /// Called when the script is enabled. Subscribes to the SceneManager.sceneLoaded event.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SendCameraData;
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the SceneManager.sceneLoaded event.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SendCameraData;
    }

    /// <summary>
    /// Called when a scene is loaded. Starts a coroutine to register the camera with a delay.
    /// </summary>
    /// <param name="scene">The scene that was loaded.</param>
    /// <param name="mode">The mode used to load the scene.</param>
    private void SendCameraData(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RegisterCameraAfterDelay());
    }

    /// <summary>
    /// Coroutine that waits for one frame before registering the camera with the CameraController.
    /// </summary>
    /// <returns>An enumerator for the coroutine.</returns>
    private IEnumerator RegisterCameraAfterDelay()
    {
        // Wait for one frame to ensure that the scene is fully loaded
        yield return null;

        Camera camera = GetComponent<Camera>();

        if (camera != null)
            CameraController.Instance.AddCamera(cameraName, camera);
    }
}
