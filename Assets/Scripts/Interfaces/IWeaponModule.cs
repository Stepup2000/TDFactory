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
    /// Gets or sets the maximum amount of ammo.
    /// </summary>
    int maxAmmoCount { get; set; }

    /// <summary>
    /// Gets or sets the duration of the reload.
    /// </summary>
    float reloadDuration { get; set; }

    /// <summary>
    /// Gets or sets the cooldown time between weapon shots.
    /// </summary>
    float shootCooldown { get; set; }

    /// <summary>
    /// Gets or sets the directional offset (in degrees) applied to each bullet.
    /// </summary>
    float recoilAmount { get; set; }
}
