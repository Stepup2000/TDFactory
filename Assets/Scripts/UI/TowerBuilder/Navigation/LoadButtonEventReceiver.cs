using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadButtonEventReceiver : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus<ToggleLoadButtonsEvent>.Subscribe(ToggleActive);
    }

    private void OnDisable()
    {
        EventBus<ToggleLoadButtonsEvent>.UnSubscribe(ToggleActive);
    }

    private void ToggleActive(ToggleLoadButtonsEvent newEvent)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(newEvent.trueOrFalse);
        }
    }
}
