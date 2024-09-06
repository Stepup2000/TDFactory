using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for weapon modules that can be equipped to a tower or character.
/// </summary>
public interface IWeapon : IModule
{
    /// <summary>
    /// Gets or sets the damage multiplier applied by the weapon.
    /// </summary>
    float damageMultiplier { get; set; }

    /// <summary>
    /// Gets or sets the cooldown time between weapon shots.
    /// </summary>
    float shootCooldown { get; set; }
}
