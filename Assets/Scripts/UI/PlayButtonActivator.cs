using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButtonActivator : MonoBehaviour
{
    private Button button;

    /// <summary>
    /// Initializes the button component and checks for available blueprints when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        button = GetComponent<Button>();
        CheckForBlueprint();
    }

    /// <summary>
    /// Checks if a blueprint is available and enables or disables the button accordingly.
    /// </summary>
    public void CheckForBlueprint()
    {
        ToggleButton(IsBlueprintAvailable());
    }

    /// <summary>
    /// Determines if any blueprints (towers) are available.
    /// </summary>
    /// <returns>True if at least one blueprint is available, otherwise false.</returns>
    private bool IsBlueprintAvailable()
    {
        var towers = PlayerDataManager.Instance?.GetAllTowers();
        if (towers == null)
            return false;

        if (towers.Count == 0)
            return false;

        return true;
    }

    /// <summary>
    /// Sets the interactability of the button based on the given state.
    /// </summary>
    /// <param name="isEnabled">If true, enables the button; otherwise, disables it.</param>
    private void ToggleButton(bool isEnabled)
    {
        button.interactable = isEnabled;
    }
}

