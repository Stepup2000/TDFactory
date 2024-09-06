using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for modules that can be attached to a tower.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Gets or sets the cost of the module.
    /// </summary>
    int cost { get; set; }

    /// <summary>
    /// Gets or sets the prefab of the module.
    /// </summary>
    GameObject modulePrefab { get; set; }

    /// <summary>
    /// Gets or sets the sound clip played when the module is placed.
    /// </summary>
    AudioClip placementSoundClip { get; set; }

    /// <summary>
    /// Sets the parent tower for the module.
    /// </summary>
    /// <param name="newTower">The tower to which the module will be attached.</param>
    void SetParentTower(Tower newTower);
}
