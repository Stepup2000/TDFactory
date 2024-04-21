using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class CameraObject : MonoBehaviour
{
    [SerializeField] private string cameraName;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SendCameraData;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SendCameraData;
    }

    private void SendCameraData(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RegisterCameraAfterDelay());
    }

    private IEnumerator RegisterCameraAfterDelay()
    {
        // Wait for one frame
        yield return null;

        // Retrieve the Camera component
        Camera camera = GetComponent<Camera>();

        // Check if the Camera component exists
        if (camera != null)
        {
            // Add the Camera to the CameraController after the delay
            CameraController.Instance.AddCamera(cameraName, camera);
        }
    }
}
