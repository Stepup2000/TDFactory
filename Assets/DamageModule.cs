using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageModule : MonoBehaviour, IStatModifier
{
    [SerializeField] private Tower _parentTower;

    public Tower parentTower
    {
        get { return _parentTower; }
        set { _parentTower = value; }
    }

    private float _damageModifier = 1f;

    private void Start()
    {
        ExecuteModule();
    }

    public void ExecuteModule()
    {
        Dictionary<string, float> modifier = new Dictionary<string, float>();
        modifier[Tower.DAMAGE_STAT] = _damageModifier;
        _parentTower.ModifyStats(modifier);
    }
}
