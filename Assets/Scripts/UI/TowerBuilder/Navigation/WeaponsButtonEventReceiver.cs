using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsButtonEventReceiver : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Subscribe(ToggleActive);
    }

    private void OnDisable()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.UnSubscribe(ToggleActive);
    }

    private void ToggleActive(ToggleWeaponModuleButtonsEvent newEvent)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(newEvent.trueOrFalse);
        }
    }
}
