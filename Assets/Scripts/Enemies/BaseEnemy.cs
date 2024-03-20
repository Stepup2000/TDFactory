using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable
{
    //Health observarable event for the healthbar
    public delegate void ValueChangeEvent(float newValue, float maxValue);
    public event ValueChangeEvent OnHealthChanged;

    [field: SerializeField] public float maxHealth { get; set; }
    [field: SerializeField] public int spawnCost { get; private set; }
    [field: SerializeField] public float heldMoney { get; private set; }
    public float currentHealth { get; private set; }
    public float travelledDistance { get; private set; }
    private Transform[] _myPath;
    private IMoveable _myMovement;

    public void Initialize(Transform[] pPath)
    {
        _myPath = pPath;
        TryGetComponent<IMoveable>(out _myMovement);
        _myMovement?.Initialize(_myPath);
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Damage is negative, setting it to 1");
            amount = 1;
        }

        currentHealth -= amount;
        if (IsStillAlive() == false) Death();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public bool IsStillAlive() => currentHealth > 0;

    protected void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EndPoint>(out EndPoint endPoint))
        {
            Death();
        }
    }

    public void Death()
    {
        EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(heldMoney));
        Destroy(gameObject);
    }
}
