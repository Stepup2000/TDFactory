using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private Currency _goldCurrency;
    [SerializeField] private Tower _towerPrefab;
    [SerializeField] private DraggableTower _draggablePrefab;
    private DraggableTower createdDraggable = null;

    private void Awake()
    {
        _goldCurrency = Resources.Load<Currency>("GoldCurrency");
    }

    public void TryBuyTower()
    {
        if (_draggablePrefab != null && _towerPrefab != null && _goldCurrency != null)
        {
            float cost = _towerPrefab.GetStats(Tower.PRICE_STAT);
            if (MoneyController.Instance.CanAfford(cost))
            {
                if (createdDraggable == null)
                {
                    createdDraggable = Instantiate<DraggableTower>(_draggablePrefab, transform.position, Quaternion.identity);
                    createdDraggable.SetTowerPrefab(_towerPrefab);
                    EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(cost));
                }
                else Destroy(createdDraggable.gameObject);
            }            
        }
    }
}
