using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatModifier
{
    Tower parentTower { get; set; }
    void ApplyMod();
}
