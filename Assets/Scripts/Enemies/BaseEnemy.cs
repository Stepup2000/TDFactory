using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IDamageable
{
    [field: SerializeField] public int maxHealth { get; private set; }
    [field: SerializeField] public int spawnCost { get; private set; }
    public int currentHealth { get; private set; }
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

    protected void Move()
    {
        _myMovement?.TryMove();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        Move();
    }
}
