using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BaseBullet
{
    /// <summary>
    /// List of enemies that have been recently hit by the laser.
    /// </summary>
    private List<IDamageable> _recentlyHitEnemies = new List<IDamageable>();

    /// <summary>
    /// Handles the collision with other objects. Applies damage to enemies if they haven't been hit recently.
    /// </summary>
    /// <param name="other">The collider of the object this laser interacts with.</param>
    public override void HandleTriggerCollision(Collider other)
    {
        if (other.TryGetComponent(out IDamageable foundDamageAble))
        {
            // Check if the enemy has not been hit recently
            if (!_recentlyHitEnemies.Contains(foundDamageAble))
            {
                // Check if the enemy is still alive
                if (foundDamageAble.IsStillAlive())
                {
                    foundDamageAble.TakeDamage(damage); // Apply damage to the enemy
                    _recentlyHitEnemies.Add(foundDamageAble); // Add to the list of recently hit enemies
                    // Optionally, remove from the list after a delay (currently commented out)
                    // StartCoroutine(RemoveFromHitList(foundDamageAble, _hitDelay));
                }
            }
        }
    }

    /// <summary>
    /// Moves the laser. This method is overridden but currently not implemented.
    /// </summary>
    public override void Move()
    {
        // Implement laser movement logic here if needed
    }

    /*
    /// <summary>
    /// Removes an enemy from the recently hit list after a specified delay.
    /// </summary>
    /// <param name="damageable">The enemy to remove from the hit list.</param>
    /// <param name="delay">The delay before removing the enemy from the list.</param>
    /// <returns>IEnumerator for coroutine.</returns>
    private IEnumerator RemoveFromHitList(IDamageable damageable, float delay)
    {
        yield return new WaitForSeconds(delay);
        _recentlyHitEnemies.Remove(damageable);
    }
    */
}
