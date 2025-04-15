using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffect
{
    /// <summary>
    /// Gets or sets the damage applied by the effects.
    /// </summary>
    float damage { get; set; }

    /// <summary>
    /// Executes the code related to the effect.
    /// </summary>
    void ExecuteEffect(BaseEnemy targetEnemy);
}
