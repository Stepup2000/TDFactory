using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour, IBullet
{
    [field: SerializeField] public float speed { get; set; }
    [field: SerializeField] public float damage { get; set; }
    [SerializeField] private float _lifeTime = 2;
    public void Initialize(float pDamage)
    {
        damage = pDamage;
        Invoke(nameof(DestroySelf), _lifeTime);
    }

    public virtual void Move()
    {
        // Get the forward direction of the object
        Vector3 direction = transform.forward;

        // Move the object in the forward direction
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTriggerCollision(other);
    }

    public virtual void HandleTriggerCollision(Collider other)
    {
        if (other.TryGetComponent(out IDamageable foundDamageAble))
        {
            if (foundDamageAble.IsStillAlive() == true)
            {
                foundDamageAble.TakeDamage(damage);
                DestroySelf();
            }
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        Move();
    }
}
