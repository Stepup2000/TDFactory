using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private Currency _goldCurrency;  // Reference to the currency used for purchasing towers.
    [SerializeField] private DraggableTower _draggablePrefab;  // Prefab for the draggable tower to be instantiated.
    [SerializeField] private int _towerNumber = 0;  // Index of the tower in the available list.

    private DraggableTower createdDraggable = null;  // Reference to the currently created draggable tower.
    private int _towerCost = 0;  // Cost of the tower.

    private void Awake()
    {
        // Load the gold currency resource.
        _goldCurrency = Resources.Load<Currency>("GoldCurrency");

        // Retrieve the list of available tower blueprints.
        List<TowerBlueprint> allTowers = PlayerDataManager.Instance.GetAllTowers();
        TowerBlueprint towerBlueprint = null;

        // Ensure allTowers is not null and has elements.
        if (allTowers != null && allTowers.Count > 0)
        {
            // Ensure the towerNumber is within a valid range.
            if (_towerNumber >= 0 && _towerNumber < allTowers.Count)
            {
                // Get the tower blueprint at the specified index.
                towerBlueprint = allTowers[_towerNumber];
            }
            else
            {
                Debug.LogWarning("Tower number out of range!");
                return;
            }
        }
        else
        {
            Debug.LogWarning("No towers available!");
            return;
        }

        // Calculate the cost of the tower by summing the costs of its parts.
        int newCost = 0;
        foreach (TowerPart part in towerBlueprint.allTowerParts)
        {
            newCost += part.ModuleCost;
        }
        _towerCost = newCost;
    }

    /// <summary>
    /// Attempts to purchase and place the tower if the player can afford it.
    /// </summary>
    public void TryBuyTower()
    {
        if (_draggablePrefab != null && _goldCurrency != null)
        {
            if (MoneyController.Instance.CanAfford(_towerCost))
            {
                // If no draggable tower has been created yet, instantiate one.
                if (createdDraggable == null)
                {
                    createdDraggable = Instantiate<DraggableTower>(_draggablePrefab, transform.position, Quaternion.identity);
                    // Deduct the cost of the tower from the player's money.
                    EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(-_towerCost, transform.position));
                    // Change the camera view to the top camera.
                    CameraController.Instance.ChangeCamera("TopCamera");
                }
                else
                {
                    // If a draggable tower has already been created, destroy it.
                    Destroy(createdDraggable.gameObject);
                }
            }
        }
    }
}
