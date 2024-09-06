using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages multiple cameras, enabling smooth transitions between them.
/// This class follows a singleton pattern to ensure only one instance is created and persists between scenes.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f; // Speed at which the camera transitions between positions and rotations

    [SerializeField] private Camera _cameraPrefab; // Prefab used to instantiate the transition camera

    /// <summary>
    /// Delegate for camera change events.
    /// </summary>
    public delegate void Triggerevent();

    /// <summary>
    /// Event triggered whenever the active camera is changed.
    /// </summary>
    public event Triggerevent OnCameraChange;

    private Dictionary<string, Camera> _allCameras = new Dictionary<string, Camera>(); // Dictionary storing all available cameras by their key names

    private Camera _currentCamera; // The currently active camera

    private Camera _targetCamera; // The target camera to transition to

    private Camera _transitionCamera; // A transition camera that handles smooth movement between cameras

    private static CameraController _instance; // Singleton instance of the CameraController

    /// <summary>
    /// Gets the singleton instance of the CameraController, creating one if it does not exist.
    /// </summary>
    public static CameraController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraController>();
                if (_instance == null)
                {
                    // If no instance exists, create one
                    GameObject singletonObject = new GameObject("CameraController");
                    _instance = singletonObject.AddComponent<CameraController>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Ensures that only one instance of CameraController exists and sets up persistent behavior across scenes.
    /// </summary>
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // Destroy this object if another instance already exists
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Subscribes to the sceneLoaded event and creates the transition camera when enabled.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ResetCameraList;
        if (_transitionCamera == null) _transitionCamera = CreateTransitionCamera();
    }

    /// <summary>
    /// Unsubscribes from the sceneLoaded event when disabled.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetCameraList;
    }

    /// <summary>
    /// Updates the camera position by moving towards the target camera each frame.
    /// </summary>
    private void Update()
    {
        MoveToTargetCamera();
    }

    /// <summary>
    /// Creates the transition camera from the prefab, sets it to inactive, and adds it to the camera list.
    /// </summary>
    /// <returns>The created transition camera or null if the prefab is missing.</returns>
    private Camera CreateTransitionCamera()
    {
        if (_cameraPrefab == null)
        {
            Debug.LogWarning("TransitionCamera prefab not found, returning null");
            return null;
        }

        Camera newCamera = Instantiate(_cameraPrefab);
        DontDestroyOnLoad(newCamera);
        newCamera.enabled = false;
        newCamera.GetComponent<AudioListener>().enabled = false;
        CameraController.Instance.AddCamera("TransitionCamera", newCamera);
        return newCamera;
    }

    /// <summary>
    /// Resets the camera list and sets the current camera to the main camera in the newly loaded scene.
    /// </summary>
    /// <param name="scene">The scene that was loaded.</param>
    /// <param name="mode">The mode in which the scene was loaded.</param>
    private void ResetCameraList(Scene scene, LoadSceneMode mode)
    {
        _allCameras.Clear();
        _currentCamera = Camera.main;
    }

    /// <summary>
    /// Adds a camera to the list of available cameras.
    /// </summary>
    /// <param name="cameraKey">Unique key to identify the camera.</param>
    /// <param name="newCamera">The camera to be added.</param>
    public void AddCamera(string cameraKey, Camera newCamera)
    {
        _allCameras[cameraKey] = newCamera;
    }

    /// <summary>
    /// Initiates a transition to the camera associated with the given key.
    /// </summary>
    /// <param name="cameraKey">Key identifying the target camera.</param>
    public void ChangeCamera(string cameraKey)
    {
        if (!_allCameras.ContainsKey(cameraKey))
        {
            Debug.LogWarning("No camera found with key: " + cameraKey);
            return;
        }

        if (_currentCamera == null)
        {
            Debug.LogWarning("No current camera found");
            return;
        }

        if (_transitionCamera == null)
        {
            Debug.LogWarning("No transition camera found, creating one");
            _transitionCamera = CreateTransitionCamera();
        }

        // Set the transition camera to the current camera's position and rotation
        _transitionCamera.transform.position = _currentCamera.transform.position;
        _transitionCamera.transform.rotation = _currentCamera.transform.rotation;
        _currentCamera = _transitionCamera;
        _targetCamera = _allCameras[cameraKey];

        ActivateCamera(_transitionCamera);
    }

    /// <summary>
    /// Moves the transition camera towards the target camera and switches to it once the transition is complete.
    /// </summary>
    private void MoveToTargetCamera()
    {
        if (_currentCamera == _transitionCamera && _targetCamera != null)
        {
            float lerpFactor = _movementSpeed * Time.deltaTime;

            // Lerp position and rotation
            _transitionCamera.transform.position = Vector3.Lerp(_transitionCamera.transform.position, _targetCamera.transform.position, lerpFactor);
            _transitionCamera.transform.rotation = Quaternion.Lerp(_transitionCamera.transform.rotation, _targetCamera.transform.rotation, lerpFactor);

            // If the transition camera is close enough to the target, finalize the transition
            if (Vector3.Distance(_transitionCamera.transform.position, _targetCamera.transform.position) < 0.1f &&
                Quaternion.Angle(_transitionCamera.transform.rotation, _targetCamera.transform.rotation) < 1f)
            {
                _currentCamera = _targetCamera;
                ActivateCamera(_targetCamera);
                _targetCamera = null;
            }
        }
    }

    /// <summary>
    /// Activates the specified camera and disables all others, triggering the camera change event.
    /// </summary>
    /// <param name="activeCamera">The camera to activate.</param>
    private void ActivateCamera(Camera activeCamera)
    {
        OnCameraChange?.Invoke();

        // Disable all cameras
        foreach (var kvp in _allCameras)
        {
            Camera camera = kvp.Value;
            camera.enabled = false;
            camera.GetComponent<AudioListener>().enabled = false;
        }

        // Enable the active camera
        if (activeCamera != null)
        {
            activeCamera.enabled = true;
            activeCamera.GetComponent<AudioListener>().enabled = true;
        }
        else
        {
            Debug.LogWarning("No active camera found, turning camera with key 'default' on");
            ActivateCamera(_allCameras.ContainsKey("default") ? _allCameras["default"] : null);
        }
    }

    /// <summary>
    /// Returns the currently active camera.
    /// </summary>
    /// <returns>The currently active camera.</returns>
    public Camera GetCurrentCamera()
    {
        return _currentCamera;
    }
}
