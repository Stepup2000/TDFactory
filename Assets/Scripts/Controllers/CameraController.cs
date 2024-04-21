using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private Camera _cameraPrefab;

    private Dictionary<string, Camera> _allCameras = new Dictionary<string, Camera>();
    private Camera _currentCamera;
    private Camera _targetCamera;
    private Camera _transitionCamera;

    private static CameraController _instance;

    public static CameraController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraController>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("CameraController");
                    _instance = singletonObject.AddComponent<CameraController>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += ResetCameraList;
        if (_transitionCamera == null) _transitionCamera = CreateTransitionCamera();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetCameraList;
    }

    private void Update()
    {
        MoveToTarget();
    }

    private Camera CreateTransitionCamera()
    {
        Camera newCamera = Instantiate<Camera>(_cameraPrefab);
        DontDestroyOnLoad(newCamera);
        newCamera.enabled = false;
        newCamera.GetComponent<AudioListener>().enabled = false;
        CameraController.Instance.AddCamera("TransitionCamera", newCamera);
        return newCamera;
    }

    private void ResetCameraList(Scene scene, LoadSceneMode mode)
    {
        _allCameras.Clear();
        _currentCamera = Camera.main;
    }

    public void AddCamera(string cameraKey, Camera newCamera)
    {
        _allCameras[cameraKey] = newCamera;
    }

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

        SetActiveCamera(_transitionCamera);

        _transitionCamera.transform.position = _currentCamera.transform.position;
        _transitionCamera.transform.rotation = _currentCamera.transform.rotation;
        _targetCamera = _allCameras[cameraKey];

        _currentCamera = _transitionCamera;
    }

    private void MoveToTarget()
    {
        if (_currentCamera == _transitionCamera && _targetCamera != null)
        {
            // Calculate the lerp factor based on movement speed and time
            float lerpFactor = _movementSpeed * Time.deltaTime;

            // Lerp position
            _transitionCamera.transform.position = Vector3.Lerp(_transitionCamera.transform.position, _targetCamera.transform.position, lerpFactor);

            // Lerp rotation
            _transitionCamera.transform.rotation = Quaternion.Lerp(_transitionCamera.transform.rotation, _targetCamera.transform.rotation, lerpFactor);

            // Check if the camera has reached close to the target position and rotation
            if (Vector3.Distance(_transitionCamera.transform.position, _targetCamera.transform.position) < 0.1f &&
                Quaternion.Angle(_transitionCamera.transform.rotation, _targetCamera.transform.rotation) < 1f)
            {
                // Set the current camera to the target camera
                _currentCamera = _targetCamera;

                // Activate the target camera
                SetActiveCamera(_targetCamera);

                // Reset the target camera to null
                _targetCamera = null;
            }
        }
    }

    private void SetActiveCamera(Camera activeCamera)
    {
        foreach (var kvp in _allCameras)
        {
            Camera camera = kvp.Value;
            camera.enabled = false;
            camera.GetComponent<AudioListener>().enabled = false;
        }

        if (activeCamera != null)
        {
            activeCamera.enabled = true;
            activeCamera.GetComponent<AudioListener>().enabled = true;
        }
        else
        {
            Debug.LogWarning("No active camera found, turning camera with key 'default' on");
            SetActiveCamera(_allCameras.ContainsKey("default") ? _allCameras["default"] : null);
        }
    }

    public Camera GetCurrentCamera()
    {
        return _currentCamera;
    }
}
