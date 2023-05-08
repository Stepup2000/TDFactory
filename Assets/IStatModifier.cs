using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatModifier : IModule
{
    Tower parentTower { get; set; }
    new void ExecuteMod();
}
