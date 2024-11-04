using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ShopButton : MonoBehaviour
{
    [SerializeField] private DraggableTower draggablePrefab;
    [SerializeField] private int towerNumber = 0;

    private Button button;
    private DraggableTower activeDragableTower;
    private List<TowerBlueprint> allTowers;
    private TowerBlueprint towerBlueprint;
    private int towerCost = 0;

    private void OnEnable()
    {
        button = GetComponent<Button>();
        EventBus<TotalMoneyChangedEvent>.Subscribe(OnTotalMoneyChanged);

        allTowers = PlayerDataManager.Instance.GetAllTowers();
        towerBlueprint = ValidateAndGetTowerBlueprint(allTowers);

        if (towerBlueprint != null)
            towerCost = CalculateTowerCost(towerBlueprint);
    }

    private void OnDisable()
    {
        EventBus<TotalMoneyChangedEvent>.UnSubscribe(OnTotalMoneyChanged);
    }

    /// <summary>
    /// Validates tower data and retrieves the tower blueprint at the specified index.
    /// </summary>
    private TowerBlueprint ValidateAndGetTowerBlueprint(List<TowerBlueprint> allTowers)
    {
        if (allTowers == null || allTowers.Count == 0)
        {
            Debug.LogWarning("No towers available!");
            return null;
        }

        if (towerNumber < 0 || towerNumber >= allTowers.Count)
        {
            Debug.LogWarning("Tower number out of range!");
            return null;
        }

        return allTowers[towerNumber];
    }

    /// <summary>
    /// Calculates the total cost of a tower by summing the costs of its parts.
    /// </summary>
    private int CalculateTowerCost(TowerBlueprint towerBlueprint)
    {
        int totalCost = 0;

        foreach (TowerPart part in towerBlueprint.allTowerParts)
        {
            totalCost += part.ModuleCost;
        }

        return totalCost;
    }

    /// <summary>
    /// Attempts to purchase and place the tower if the player can afford it.
    /// </summary>
    public void TryBuyTower()
    {
        if (draggablePrefab != null && MoneyController.Instance.CanAfford(towerCost))
        {
            // If no draggable tower has been created yet, instantiate one.
            if (activeDragableTower == null)
            {
                activeDragableTower = Instantiate<DraggableTower>(draggablePrefab, transform.position, Quaternion.identity);
                activeDragableTower.SetTowerBlueprint(towerNumber);

                EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(-towerCost, transform.position));
                CameraController.Instance.ChangeCamera("TopCamera");
            }
            else
            {
                Destroy(activeDragableTower.gameObject);
            }
        }
    }

    /// <summary>
    /// Enables or disables the button based on whether the player can afford the tower cost
    /// and there are available towers.
    /// </summary>
    private void OnTotalMoneyChanged(TotalMoneyChangedEvent moneyChangedEvent)
    {
        bool blueprintExists = towerBlueprint != null;
        bool canAfford = MoneyController.Instance.CanAfford(towerCost);
        bool hasTowersAvailable = allTowers != null && allTowers.Count > 0;

        button.interactable = canAfford && hasTowersAvailable && blueprintExists;
    }
}
