using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyButtonEventReceiver : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus<ToggleBodyModuleButtonsEvent>.Subscribe(ToggleActive);
    }

    private void OnDisable()
    {
        EventBus<ToggleBodyModuleButtonsEvent>.UnSubscribe(ToggleActive);
    }

    private void ToggleActive(ToggleBodyModuleButtonsEvent newEvent)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(newEvent.trueOrFalse);
        }
    }
}
