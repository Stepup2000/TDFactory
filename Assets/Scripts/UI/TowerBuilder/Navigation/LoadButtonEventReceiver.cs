using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadButtonEventReceiver : MonoBehaviour
{
    /// <summary>
    /// Called when the script is enabled. Subscribes to the ToggleLoadButtonsEvent.
    /// </summary>
    private void OnEnable()
    {
        EventBus<ToggleLoadButtonsEvent>.Subscribe(ToggleActive);
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the ToggleLoadButtonsEvent.
    /// </summary>
    private void OnDisable()
    {
        EventBus<ToggleLoadButtonsEvent>.UnSubscribe(ToggleActive);
    }

    /// <summary>
    /// Toggles the active state of the child objects based on the event data.
    /// </summary>
    /// <param name="newEvent">Event containing the toggle state.</param>
    private void ToggleActive(ToggleLoadButtonsEvent newEvent)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(newEvent.trueOrFalse);
        }
    }
}
