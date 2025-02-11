using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTowerNumber : ChangeLocalizedString
{
    [SerializeField] protected int towerNumber;

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateText();
    }

    /// <summary>
    /// Updates the localized text event with the new string key.
    /// </summary>
    public void UpdateText()
    {
        ChangeString("UI", "Tower", towerNumber);
    }
}
