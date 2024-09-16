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

    /// <summary>
    /// Called when the script is enabled. Sets up the health bar and subscribes to necessary events.
    /// </summary>
    private void OnEnable()
    {
        SetupHealthBar();
        if (_parent != null) _parent.OnHealthChanged += UpdateHealthbar;
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from events to avoid potential memory leaks.
    /// </summary>
    private void OnDisable()
    {
        if (_parent != null) _parent.OnHealthChanged -= UpdateHealthbar;
    }

    /// <summary>
    /// Initializes the health bar, setting the proper camera and health values.
    /// </summary>
    private void SetupHealthBar()
    {
        UpdateHealthbar(1, 1); // Initialize health bar with dummy values
    }

    /// <summary>
    /// Validates that all necessary components are properly set up. If any are missing, logs a warning and destroys the object.
    /// </summary>
    private void ValidateHealthBar()
    {
        if (_myHealthBar == null || _fillImage == null || _colorGradient == null)
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
}
