using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the display of the health bar for an enemy, ensuring it updates correctly with the enemy's health,
/// and always faces the camera.
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private BaseEnemy _parent; // Reference to the enemy whose health this bar displays.
    [SerializeField] private Slider _myHealthBar; // The slider component representing the health bar.
    [SerializeField] private Image _fillImage; // The image component representing the fill of the health bar.
    [SerializeField] private Gradient _colorGradient; // The gradient used to color the fill image based on health.

    private float _maxHealth; // The maximum health value for the enemy.
    private float _currentHealth; // The current health value of the enemy.
    private Camera _targetCamera; // The camera that the health bar should face.
    private Quaternion _originalRotation; // The original rotation of the health bar to maintain proper orientation.

    /// <summary>
    /// Called when the script is enabled. Sets up the health bar and subscribes to necessary events.
    /// </summary>
    private void OnEnable()
    {
        SetupHealthBar();
        if (_parent != null) _parent.OnHealthChanged += UpdateHealthbar;
        if (CameraController.Instance != null) CameraController.Instance.OnCameraChange += ChangeTargetCamera;
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from events to avoid potential memory leaks.
    /// </summary>
    private void OnDisable()
    {
        if (_parent != null) _parent.OnHealthChanged -= UpdateHealthbar;
        if (CameraController.Instance != null) CameraController.Instance.OnCameraChange -= ChangeTargetCamera;
    }

    /// <summary>
    /// Updates the reference to the current camera from the CameraController.
    /// </summary>
    private void ChangeTargetCamera()
    {
        if (CameraController.Instance != null)
        {
            _targetCamera = CameraController.Instance.GetCurrentCamera();
        }
    }

    /// <summary>
    /// Initializes the health bar, setting the proper camera and health values.
    /// </summary>
    private void SetupHealthBar()
    {
        ChangeTargetCamera();
        _originalRotation = transform.rotation;
        UpdateHealthbar(1, 1); // Initialize health bar with dummy values
        TurnToCamera();
    }

    /// <summary>
    /// Validates that all necessary components are properly set up. If any are missing, logs a warning and destroys the object.
    /// </summary>
    private void ValidateHealthBar()
    {
        if (_targetCamera == null || _myHealthBar == null || _fillImage == null || _colorGradient == null)
        {
            Debug.LogWarning("Health bar prefab was not set up properly");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates the health bar's value and appearance based on the new and maximum health values.
    /// </summary>
    /// <param name="newValue">The current health value.</param>
    /// <param name="maxValue">The maximum health value.</param>
    public void UpdateHealthbar(float newValue, float maxValue)
    {
        ValidateHealthBar();

        _currentHealth = newValue;
        _maxHealth = maxValue;

        _myHealthBar.maxValue = _maxHealth;
        _myHealthBar.value = _currentHealth;
        _fillImage.color = _colorGradient.Evaluate(1 - _myHealthBar.value / _maxHealth);
    }

    /// <summary>
    /// Rotates the health bar to always face the current camera.
    /// </summary>
    private void TurnToCamera()
    {
        if (_targetCamera != null)
        {
            transform.LookAt(_targetCamera.transform.position, Vector3.down);
            transform.rotation = _targetCamera.transform.rotation * _originalRotation;
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
