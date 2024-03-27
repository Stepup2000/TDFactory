using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageModule : MonoBehaviour, IStatModifier
{
    [SerializeField] private Tower _parentTower;
    [field: SerializeField] public int cost { get; set; }
    [field: SerializeField] public GameObject modulePrefab { get; set; }
    [field: SerializeField] public AudioClip placementSoundClip { get; set; }

    private float _damageModifier = 1f;

    public Tower parentTower
    {
        get { return _parentTower; }
        set { _parentTower = value; }
    }

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

    private void Start()
    {
        ExecuteModule();
    }

    public void ExecuteModule()
    {
        Dictionary<string, float> modifier = new();
        modifier[Tower.DAMAGE_STAT] = _damageModifier;
        _parentTower.ModifyStats(modifier);
    }
}
