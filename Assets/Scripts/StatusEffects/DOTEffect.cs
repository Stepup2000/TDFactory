using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Applies a damage-over-time effect to an enemy, dealing periodic damage for a set duration.
/// </summary>
public class DOTEffect : MonoBehaviour, IStatusEffect
{
    /// <summary>
    /// The amount of damage dealt per tick.
    /// </summary>
    [field: SerializeField] public float damage { get; set; }

    /// <summary>
    /// The time interval between each damage tick (in seconds).
    /// </summary>
    [field: SerializeField] public float dotCooldown { get; set; }

    /// <summary>
    /// The total duration the DoT effect lasts (in seconds).
    /// </summary>
    [field: SerializeField] public float duration { get; set; }

    /// <summary>
    /// Starts applying the DoT effect to the target enemy.
    /// </summary>
    /// <param name="targetEnemy">The enemy to apply the effect to.</param>
    public virtual void ExecuteEffect(BaseEnemy targetEnemy)
    {
        IDamageable damagable = targetEnemy.GetComponent<IDamageable>();
        if (damagable != null)
            StartCoroutine(ApplyDamageOverTime(damagable));
    }

    /// <summary>
    /// Coroutine that applies damage every dotCooldown seconds for the duration of the effect.
    /// </summary>
    /// <param name="target">The damageable target receiving the DoT effect.</param>
    protected virtual IEnumerator ApplyDamageOverTime(IDamageable target)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            target.TakeDamage(damage);
            yield return new WaitForSeconds(dotCooldown);
            elapsed += dotCooldown;
        }
    }
}
