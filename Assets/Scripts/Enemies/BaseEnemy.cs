using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for all enemy types. Handles enemy health, movement, and interactions with other game objects.
/// </summary>
public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    // Delegate and event for notifying changes in enemy health
    public delegate void ValueChangeEvent(float newValue, float maxValue);
    public event ValueChangeEvent OnHealthChanged;

    [field: SerializeField] public float maxHealth { get; set; } // Maximum health of the enemy
    [field: SerializeField] public int spawnCost { get; private set; } // Cost to spawn this enemy
    [field: SerializeField] public float heldMoney { get; private set; } // Amount of money held by the enemy
    public float currentHealth { get; private set; } // Current health of the enemy
    public float travelledDistance { get; private set; } // Distance travelled by the enemy

    private Transform[] _myPath; // Path that the enemy follows
    private IMoveable _myMovement; // Movement component of the enemy

    /// <summary>
    /// Initializes the enemy with a path to follow and sets up its movement component.
    /// </summary>
    /// <param name="pPath">Path the enemy should follow.</param>
    public void Initialize(Transform[] pPath)
    {
        _myPath = pPath;
        TryGetComponent<IMoveable>(out _myMovement);
        _myMovement?.Initialize(_myPath);
        currentHealth = maxHealth; // Reset current health to max health
    }

    /// <summary>
    /// Applies damage to the enemy and triggers death if health drops to zero or below.
    /// </summary>
    /// <param name="amount">Amount of damage to apply.</param>
    public void TakeDamage(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Damage amount is negative. Setting it to 1.");
            amount = 1;
        }

        currentHealth -= amount;
        if (!IsStillAlive())
        {
            Death();
        }
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Checks if the enemy is still alive.
    /// </summary>
    /// <returns>True if the enemy's health is above zero; otherwise, false.</returns>
    public bool IsStillAlive() => currentHealth > 0;

    /// <summary>
    /// Handles collision with an EndPoint and triggers enemy death.
    /// </summary>
    /// <param name="other">The collider that the enemy collided with.</param>
    protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EndPoint>(out EndPoint endPoint))
        {
            Death();
        }
    }

    /// <summary>
    /// Handles the death of the enemy, publishing an event and destroying the game object.
    /// </summary>
    public void Death()
    {
        EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(heldMoney));
        Destroy(gameObject);
    }
}
