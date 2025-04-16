using System.Collections;
using UnityEngine;

public class BaseStatusEffect : MonoBehaviour, IStatusEffect, IPoolableEffect
{
    [field: SerializeField] public EffectType effectType { get; set; }
    [field: SerializeField] public float initialDamage { get; set; }
    [field: SerializeField] public float duration { get; set; }

    protected IDamageable targetDamageable;

    public System.Action<BaseStatusEffect> returnToPoolCallback;

    /// <summary>
    /// Applies the effect, attaching it to the target's GameObject.
    /// </summary>
    public virtual void ApplyEffect(IDamageable target)
    {
        targetDamageable = target;

        if (target is Component c)
        {
            target.TakeDamage(initialDamage);
            StartCoroutine(EffectDurationTimer());
        }
    }

    /// <summary>
    /// Waits for the duration of the effect, then removes or disables it.
    /// </summary>
    protected virtual IEnumerator EffectDurationTimer()
    {
        yield return new WaitForSeconds(duration);
        ResetEffect();
    }

    /// <summary>
    /// Called when the effect needs to be reset.
    /// </summary>
    public virtual void ResetEffect()
    {
        returnToPoolCallback?.Invoke(this);
    }

    /// <summary>
    /// Called when the effect is spawned or activated from the pool.
    /// </summary>
    public virtual void OnSpawn() => gameObject.SetActive(true);

    /// <summary>
    /// Called when the effect is despawned or returned to the pool.
    /// </summary>
    public virtual void OnDespawn() => gameObject.SetActive(false);

}
