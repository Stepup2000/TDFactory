using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionButtonEventReceiver : MonoBehaviour
{
    /// <summary>
    /// Called when the script is enabled. Subscribes to the ToggleDetectionModuleButtonsEvent.
    /// </summary>
    private void OnEnable()
    {
        EventBus<ToggleDetectionModuleButtonsEvent>.Subscribe(ToggleActive);
    }

    /// <summary>
    /// Called when the script is disabled. Unsubscribes from the ToggleDetectionModuleButtonsEvent.
    /// </summary>
    private void OnDisable()
    {
        EventBus<ToggleDetectionModuleButtonsEvent>.UnSubscribe(ToggleActive);
    }

    /// <summary>
    /// Toggles the active state of the child objects based on the event data.
    /// </summary>
    /// <param name="newEvent">Event containing the toggle state.</param>
    private void ToggleActive(ToggleDetectionModuleButtonsEvent newEvent)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(newEvent.trueOrFalse);
        }
    }
}
