using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionButtonEventReceiver : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus<ToggleDetectionModuleButtonsEvent>.Subscribe(ToggleActive);
    }

    private void OnDisable()
    {
        EventBus<ToggleDetectionModuleButtonsEvent>.UnSubscribe(ToggleActive);
    }

    private void ToggleActive(ToggleDetectionModuleButtonsEvent newEvent)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(newEvent.trueOrFalse);
        }
    }
}
