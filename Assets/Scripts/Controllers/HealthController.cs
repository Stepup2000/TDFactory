using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's health, ensuring only one instance exists throughout the game.
/// Subscribes to health change events and updates the current health accordingly.
/// </summary>
public class HealthController : MonoBehaviour
{
    [SerializeField] private Currency _currency; // Currency reference (if needed for further implementation)
    private static HealthController instance; // Singleton instance of HealthController
    private float _currentHealth; // Current health of the player

    /// <summary>
    /// Provides access to the singleton instance of HealthController.
    /// Ensures only one instance exists and persists across scenes.
    /// </summary>
    public static HealthController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HealthController>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("HealthController");
                    instance = singletonObject.AddComponent<HealthController>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Subscribes to health change events when the component is enabled.
    /// </summary>
    private void OnEnable()
    {
        EventBus<ChangeHealthEvent>.Subscribe(ChangeHealth);
    }

    /// <summary>
    /// Unsubscribes from health change events when the component is disabled.
    /// </summary>
    private void OnDisable()
    {
        EventBus<ChangeHealthEvent>.UnSubscribe(ChangeHealth);
    }

    /// <summary>
    /// Handles health changes based on the received event.
    /// Updates the current health and publishes the new total health.
    /// </summary>
    /// <param name="pEvent">The ChangeHealthEvent containing the amount to change the health by.</param>
    private void ChangeHealth(ChangeHealthEvent pEvent)
    {
        _currentHealth += pEvent.amount;
        if (_currentHealth <= 0) OnDefeat();
        EventBus<TotalHealthChangedEvent>.Publish(new TotalHealthChangedEvent(_currentHealth));
    }

    /// <summary>
    /// Checks if the game is over based on the current health.
    /// </summary>
    /// <returns>True if the current health is less than or equal to zero, otherwise false.</returns>
    public bool IsGameOver()
    {
        return _currentHealth <= 0;
    }

    private void OnDefeat()
    {
        LevelManager.LoadLevel("EndScreen");
    }
}
