using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffect
{
    /// <summary>
    /// Gets or sets the damage applied by the effects.
    /// </summary>
    float initialDamage { get; set; }

    /// <summary>
    /// Gets or sets the duration of the effect.
    /// </summary>
    float duration { get; set; }

    /// <summary>
    /// Executes the code related to the effect.
    /// </summary>
    void ApplyEffect(IDamageable target);

    /// <summary>
    /// Executes when the effects duration is over.
    /// </summary>
    void ResetEffect();
}
