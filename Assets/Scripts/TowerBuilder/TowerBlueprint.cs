using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlueprint
{
    public List<TowerPart> allTowerParts;
    public int buildcost;

    public TowerBlueprint(List<TowerPart> pAllTowerParts, int pBuildCost)
    {
        allTowerParts = pAllTowerParts;
        buildcost = pBuildCost;
    }
}
