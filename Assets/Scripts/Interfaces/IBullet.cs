using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a bullet with damage and speed attributes.
/// </summary>
public interface IBullet
{
    /// <summary>
    /// Gets or sets the damage value of the bullet.
    /// </summary>
    float damage { get; set; }

    /// <summary>
    /// Gets or sets the speed of the bullet.
    /// </summary>
    float speed { get; set; }

    /// <summary>
    /// Initializes the bullet with a specified damage value.
    /// </summary>
    /// <param name="pDamage">The damage value to initialize the bullet with.</param>
    void Initialize(float pDamage);
}
