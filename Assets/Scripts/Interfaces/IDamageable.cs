using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for objects that can receive damage and have health management.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Gets or sets the maximum health of the object.
    /// </summary>
    float maxHealth { get; set; }

    /// <summary>
    /// Applies damage to the object.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    void TakeDamage(float damage);

    /// <summary>
    /// Determines if the object is still alive.
    /// </summary>
    /// <returns>True if the object is still alive; otherwise, false.</returns>
    bool IsStillAlive();

    /// <summary>
    /// Handles the object's death process.
    /// </summary>
    void Death();
}
