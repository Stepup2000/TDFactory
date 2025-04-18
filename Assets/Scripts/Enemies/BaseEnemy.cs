using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract base class for all enemy types. Handles enemy health, movement, and interactions with other game objects.
/// </summary>
public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    // Delegate and event for notifying changes in enemy health
    /// <summary>
    /// Delegate for value change events, providing the new and max values.
    /// </summary>
    /// <param name="newValue">The new health value.</param>
    /// <param name="maxValue">The maximum health value.</param>
    public delegate void ValueChangeEvent(float newValue, float maxValue);

    /// <summary>
    /// Event triggered when health changes.
    /// </summary>
    public event ValueChangeEvent OnHealthChanged;

    [field: SerializeField, Tooltip("The maximum health of the enemy.")]
    public float maxHealth { get; set; }

    [field: SerializeField, Tooltip("An multiplier to make the enemies have more health every wave")]
    public float healthMultiplier { get; set; }

    [field: SerializeField, Tooltip("The cost to spawn the enemy.")]
    public int spawnCost { get; private set; }

    [field: SerializeField, Tooltip("The amount of money the enemy holds when defeated.")]
    public float heldMoney { get; private set; }

    [Tooltip("The current health of the enemy.")]
    public float currentHealth { get; private set; }

    [Tooltip("The total distance the enemy has travelled.")]
    public float travelledDistance { get; private set; }


    private Transform[] _myPath;
    private IMoveable _myMovement;

    /// <summary>
    /// Initializes the enemy with a path to follow and sets up its movement component.
    /// </summary>
    /// <param name="pPath">Path the enemy should follow.</param>
    public void Initialize(Transform[] pPath)
    {
        maxHealth *= Mathf.Pow(healthMultiplier, WaveController.Instance.GetWaveIndex());
        currentHealth = maxHealth;
        _myPath = pPath;
        TryGetComponent<IMoveable>(out _myMovement);
        EffectController.Instance.ApplyEffect(EffectType.Poison, this);
        _myMovement?.Initialize(_myPath);
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
        Vector3 offset = new Vector3(0, 1, 0);
        EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(heldMoney, transform.position + offset));
        Destroy(gameObject);
    }
}
