using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPart
{
    public GameObject module;
    public int moduleCost;
    public Vector3 position;
    public Quaternion rotation;
    public TowerPart(GameObject pModule, int pCost, Vector3 pPosition, Quaternion pRotation)
    {
        module = pModule;
        moduleCost = pCost;
        position = pPosition;
        rotation = pRotation;
    }
}
