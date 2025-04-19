public interface IStatusEffect
{
    /// <summary>
    /// The type of the effect.
    /// </summary>
    EffectType effectType { get; }

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
