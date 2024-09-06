using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for modules that modify the stats of a tower.
/// </summary>
public interface IStatModifier : IModule
{
    /// <summary>
    /// Gets or sets the tower that this stat modifier is associated with.
    /// </summary>
    Tower parentTower { get; set; }
}
