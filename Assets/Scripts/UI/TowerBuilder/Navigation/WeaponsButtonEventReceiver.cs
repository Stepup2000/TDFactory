using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsButtonEventReceiver : MonoBehaviour
{
    /// <summary>
    /// Called when the script is enabled. Subscribes to the ToggleWeaponModuleButtonsEvent.
    /// </summary>
    private void OnEnable()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Subscribe(ToggleActive);
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the ToggleWeaponModuleButtonsEvent.
    /// </summary>
    private void OnDisable()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.UnSubscribe(ToggleActive);
    }

    /// <summary>
    /// Toggles the active state of the child objects based on the event data.
    /// </summary>
    /// <param name="newEvent">Event containing the toggle state.</param>
    private void ToggleActive(ToggleWeaponModuleButtonsEvent newEvent)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(newEvent.trueOrFalse);
        }
    }
}
