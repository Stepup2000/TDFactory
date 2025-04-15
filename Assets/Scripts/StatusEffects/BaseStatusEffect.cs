using System.Collections;
using UnityEngine;

public class BaseStatusEffect : MonoBehaviour, IStatusEffect
{
    [field: SerializeField] public float initialDamage { get; set; }
    [field: SerializeField] public float duration { get; set; }

    protected IDamageable targetDamageable;

    /// <summary>
    /// Applies the effect, attaching it to the target's GameObject.
    /// </summary>
    public virtual void ApplyEffect(IDamageable target)
    {
        targetDamageable = target;

        if (targetDamageable is Component component)
        {
            transform.SetParent(component.gameObject.transform);
            target?.TakeDamage(initialDamage);
            StartCoroutine(EffectDurationTimer());
        }
        else
        {
            Debug.LogWarning("Target is not a Component, cannot attach effect.");
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
        Destroy(this);
    }
}
