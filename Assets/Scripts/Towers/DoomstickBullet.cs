using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoomstickBullet : BaseBullet
{
    /// <summary>
    /// Handles logic when the bullet collides with an object.
    /// If the object is damageable, applies damage and randomly applies an effect.
    /// </summary>
    /// <param name="other">The collider of the object hit by the bullet.</param>
    public override void HandleTriggerCollision(Collider other)
    {
        if (other.TryGetComponent(out IDamageable foundDamageAble))
        {
            if (foundDamageAble.IsStillAlive())
            {
                foundDamageAble.TakeDamage(damage);

                EffectType randomEffect = GetRandomEffect();
                EffectController.Instance.ApplyEffect(randomEffect, foundDamageAble);

                DestroySelf();
            }
        }
    }

    /// <summary>
    /// Returns a randomly selected effect type from the EffectType enum.
    /// </summary>
    private EffectType GetRandomEffect()
    {
        var effectTypes = System.Enum.GetValues(typeof(EffectType));
        EffectType selectedEffect = (EffectType)effectTypes.GetValue(Random.Range(0, effectTypes.Length));

        return selectedEffect;
    }
}
