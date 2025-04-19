using UnityEngine;
using System.Collections;

/// <summary>
/// Applies a damage-over-time effect to an enemy, dealing periodic damage for a set duration.
/// </summary>
public class DOTEffect : BaseStatusEffect
{
    /// <summary>
    /// The total duration the DoT effect lasts (in seconds).
    /// </summary>
    [field: SerializeField] public float dotDamage { get; set; }

    /// <summary>
    /// The time interval between each damage tick (in seconds).
    /// </summary>
    [field: SerializeField] public float dotCooldown { get; set; }

    /// <summary>
    /// Starts applying the DoT effect to the target enemy.
    /// </summary>
    /// <param name="targetEnemy">The enemy to apply the effect to.</param>
    public override void ApplyEffect(IDamageable target)
    {
        base.ApplyEffect(target);
        if (target != null)
            StartCoroutine(ApplyDamageOverTime(target));
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
            target?.TakeDamage(dotDamage);
            yield return new WaitForSeconds(dotCooldown);
            elapsed += dotCooldown;
        }
    }
}
