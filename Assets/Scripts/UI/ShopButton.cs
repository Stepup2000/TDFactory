using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButton : MonoBehaviour
{
    [SerializeField] Currency _goldCurrency;
    [SerializeField] Tower _towerPrefab;

    private Button _button;

    private void Awake()
    {
        _goldCurrency = Resources.Load<Currency>("GoldCurrency");
        TryGetComponent(out _button);
    }

    public void TryBuyTower()
    {
        if (_towerPrefab != null && _goldCurrency != null)
        {
            float cost = _towerPrefab.GetStats(Tower.PRICE_STAT);
            if (MoneyController.Instance.CanAfford(cost))
            {
                Instantiate<Tower>(_towerPrefab, transform.position, Quaternion.identity);
                EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(cost));
            }
        }
    }
}
