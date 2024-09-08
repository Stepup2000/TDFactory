using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPart
{
    /// <summary>
    /// The module GameObject that represents the tower part.
    /// </summary>
    public GameObject Module { get; set; }

    /// <summary>
    /// The cost of the module.
    /// </summary>
    public int ModuleCost { get; set; }

    /// <summary>
    /// The position of the module in the game world.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The rotation of the module in the game world.
    /// </summary>
    public Quaternion Rotation { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TowerPart"/> class with the specified parameters.
    /// </summary>
    /// <param name="module">The GameObject module.</param>
    /// <param name="moduleCost">The cost of the module.</param>
    /// <param name="position">The world position of the module.</param>
    /// <param name="rotation">The world rotation of the module.</param>
    public TowerPart(GameObject module, int moduleCost, Vector3 position, Quaternion rotation)
    {
        Module = module;
        ModuleCost = moduleCost;
        Position = position;
        Rotation = rotation;
    }
}
