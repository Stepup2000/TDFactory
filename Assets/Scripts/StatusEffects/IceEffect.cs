using UnityEngine;
using System.Collections;

/// <summary>
/// Applies a damage-over-time effect to an enemy, dealing periodic damage for a set duration.
/// </summary>
public class IceEffect : BaseStatusEffect
{
    /// <summary>
    /// The amount the target will be slower (in percentage).
    /// </summary>
    [Range(0, 100)]
    [SerializeField] private int freezeAmount;

    private float appliedSpeedReduction;
    private IMoveable targetMoveable;

    /// <summary>
    /// Starts applying the DoT effect to the target enemy.
    /// </summary>
    /// <param name="targetEnemy">The enemy to apply the effect to.</param>
    public override void ApplyEffect(IDamageable target)
    {
        base.ApplyEffect(target);

        if (target is Component c)
        {
            c.transform.TryGetComponent<IMoveable>(out IMoveable moveable);
            targetMoveable = moveable;
            if (moveable != null)
            {
                float targetSpeed = moveable.speed;
                float speedReduction = -(targetSpeed / 100) * freezeAmount;
                moveable.AlterSpeed(speedReduction);
                appliedSpeedReduction = speedReduction;
            }
        }
    }

    /// <summary>
    /// Reapplies the speed to the target moveable.
    /// </summary>
    private void ReapplySpeed()
    {
        if (targetMoveable != null)
            targetMoveable.AlterSpeed(-appliedSpeedReduction);
    }

    /// <summary>
    /// Reset the values so that the object can be reused.
    /// </summary>
    private void ResetValues()
    {
        appliedSpeedReduction = 0;
        targetMoveable = null;
    }

    /// <summary>
    /// Called when the object gets returned to the pool.
    /// </summary>
    public override void OnDespawn()
    {
        base.OnDespawn();
        ReapplySpeed();
        ResetValues();
    }
}
