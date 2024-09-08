using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a blueprint for a tower, including its parts and build cost.
/// </summary>
public class TowerBlueprint
{
    public List<TowerPart> allTowerParts; // The list of all parts that make up the tower.

    public int buildcost; // The cost to build the tower.

    /// <summary>
    /// Initializes a new instance of the <see cref="TowerBlueprint"/> class with specified tower parts and build cost.
    /// </summary>
    /// <param name="pAllTowerParts">A list of <see cref="TowerPart"/> representing the components of the tower.</param>
    /// <param name="pBuildCost">The cost required to build the tower.</param>
    public TowerBlueprint(List<TowerPart> pAllTowerParts, int pBuildCost)
    {
        allTowerParts = pAllTowerParts; // Assigns the provided list of tower parts to the allTowerParts field.
        buildcost = pBuildCost; // Assigns the provided build cost to the buildcost field.
    }
}
