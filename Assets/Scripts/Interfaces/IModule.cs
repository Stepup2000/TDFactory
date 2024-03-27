using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModule
{
    int cost { get; set; }
    GameObject modulePrefab { get; set; }
    AudioClip placementSoundClip { get; set; }
    void SetParentTower(Tower newTower);
}
