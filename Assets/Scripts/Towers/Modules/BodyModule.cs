using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyModule : MonoBehaviour, IModule
{
    private Tower _parentTower;
    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public GameObject modulePrefab { get; set; }


    private void OnEnable()
    {
        EventBus<RequestModuleDataEvent>.Subscribe(SendModuleData);
    }

    private void OnDisable()
    {
        EventBus<RequestModuleDataEvent>.UnSubscribe(SendModuleData);
    }

    private void SendModuleData(RequestModuleDataEvent requestModuleDataEvent)
    {
        TowerBuilder.Instance.PlaceModule(gameObject, modulePrefab);
    }

    public void SetParentTower(Tower newTower)
    {
        _parentTower = newTower;
    }
}
