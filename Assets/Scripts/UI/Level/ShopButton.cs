using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private Currency _goldCurrency;
    [SerializeField] private DraggableTower _draggablePrefab;

    [SerializeField] private int _towerNumber = 0;

    private DraggableTower createdDraggable = null;

    private int _towerCost = 0;

    private void Awake()
    {
        _goldCurrency = Resources.Load<Currency>("GoldCurrency");
        List<TowerBlueprint> allTowers = PlayerDataManager.Instance.GetAllTowers();
        TowerBlueprint towerBlueprint = null;

        // Check if allTowers is not null and contains elements
        if (allTowers != null && allTowers.Count > 0)
        {
            // Check if _towerNumber is within the valid range
            if (_towerNumber >= 0 && _towerNumber < allTowers.Count)
            {
                // Access the tower at _towerNumber
                towerBlueprint = allTowers[_towerNumber];
            }
            else
            {
                // Handle the out-of-range error here, for example:
                Debug.LogWarning("Tower number out of range!");
                return;
            }
        }
        else
        {
            // Handle the case where allTowers is null or empty
            Debug.LogWarning("No towers available!");
            return;
        }

        int newCost = 0;
        foreach(TowerPart part in towerBlueprint.allTowerParts)
        {
            newCost += part.ModuleCost;
        }
        _towerCost = newCost;
    }

    public void TryBuyTower()
    {
        if (_draggablePrefab != null && _goldCurrency != null)
        {
            if (MoneyController.Instance.CanAfford(_towerCost))
            {
                if (createdDraggable == null)
                {
                    createdDraggable = Instantiate<DraggableTower>(_draggablePrefab, transform.position, Quaternion.identity);
                    EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(-_towerCost));
                    CameraController.Instance.ChangeCamera("TopCamera");
                }
                else Destroy(createdDraggable.gameObject);
            }            
        }
    }
}
