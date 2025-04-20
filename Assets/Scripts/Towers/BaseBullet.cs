using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour, IBullet
{
    /// <summary>
    /// The speed at which the bullet travels.
    /// </summary>
    [field: SerializeField] public float speed { get; set; }

    /// <summary>
    /// The damage value the bullet inflicts upon collision.
    /// </summary>
    [field: SerializeField] public float damage { get; set; }

    [SerializeField] private float _lifeTime = 2;

    /// <summary>
    /// Initializes the bullet with a given damage value and sets a timer to destroy it after a certain lifetime.
    /// </summary>
    /// <param name="pDamage">The damage value to assign to the bullet.</param>
    public void Initialize(float pDamage)
    {
        damage = pDamage;
        Invoke(nameof(DestroySelf), _lifeTime);
    }

    /// <summary>
    /// Moves the bullet in the forward direction based on its speed.
    /// </summary>
    public virtual void Move()
    {
        Vector3 direction = transform.forward;
        transform.position += direction * speed * Time.deltaTime;
    }

    /// <summary>
    /// Handles collision with other objects via trigger.
    /// </summary>
    /// <param name="other">The collider of the object the bullet has collided with.</param>
    protected void OnTriggerEnter(Collider other)
    {
        HandleTriggerCollision(other);
    }

    /// <summary>
    /// Handles logic when the bullet collides with an object.
    /// If the object is damageable, applies damage and destroys the bullet.
    /// </summary>
    /// <param name="other">The collider of the object hit by the bullet.</param>
    public virtual void HandleTriggerCollision(Collider other)
    {
        if (other.TryGetComponent(out IDamageable foundDamageAble))
        {
            if (foundDamageAble.IsStillAlive())
            {
                foundDamageAble.TakeDamage(damage);
                DestroySelf();
            }
        }
    }

    /// <summary>
    /// Destroys the bullet by removing it from the game.
    /// </summary>
    protected void DestroySelf()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Updates the bullet's position every frame by calling the Move method.
    /// </summary>
    protected void Update()
    {
        Move();
    }
}
