using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player data related to tower inventory and save slots.
/// Implements a singleton pattern to ensure only one instance exists and persists across scenes.
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private int _amountOfSaveslots = 3; // Number of save slots for towers
    private List<TowerBlueprint> allTowers = new List<TowerBlueprint>(); // List to hold tower blueprints
    private static PlayerDataManager _instance; // Singleton instance of PlayerDataManager

    /// <summary>
    /// Provides access to the singleton instance of PlayerDataManager.
    /// Ensures only one instance exists and persists across scenes.
    /// </summary>
    public static PlayerDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerDataManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("PlayerDataManager");
                    _instance = singletonObject.AddComponent<PlayerDataManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    /// <summary>
    /// Initializes the instance and ensures it persists across scenes.
    /// </summary>
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Adds a tower blueprint to the inventory at the specified slot number.
    /// Ensures the list has enough capacity and elements to accommodate the slot number.
    /// </summary>
    /// <param name="tower">The tower blueprint to add.</param>
    /// <param name="towerNumber">The index of the slot to add the tower to.</param>
    public void AddTowerToInventory(TowerBlueprint tower, int towerNumber)
    {
        // Ensure the list has enough capacity to accommodate towerNumber
        if (allTowers == null)
        {
            allTowers = new List<TowerBlueprint>(_amountOfSaveslots + 1);
        }
        else if (allTowers.Count <= _amountOfSaveslots)
        {
            int additionalCapacity = _amountOfSaveslots - allTowers.Count + 1;
            allTowers.Capacity = allTowers.Count + additionalCapacity;
        }

        // Ensure allTowers has enough elements to accommodate towerNumber
        while (allTowers.Count <= towerNumber)
        {
            allTowers.Add(null);
        }

        // Add the tower at the specified towerNumber location
        allTowers[towerNumber] = tower;
    }

    /// <summary>
    /// Attempts to remove a tower from the inventory at the specified slot number.
    /// Logs warnings if the list is null or the tower is not found.
    /// </summary>
    /// <param name="towerNumber">The index of the slot to remove the tower from.</param>
    public void TryRemoveTowerFromInventory(int towerNumber)
    {
        if (allTowers == null)
        {
            Debug.LogWarning("Tower list not found");
            return;
        }
        else if (towerNumber < 0 || towerNumber >= allTowers.Count || allTowers[towerNumber] == null)
        {
            Debug.LogWarning("Tower not found");
            return;
        }
        allTowers[towerNumber] = null;
    }

    /// <summary>
    /// Returns the list of all tower blueprints in the inventory.
    /// Logs a warning if no towers are found.
    /// </summary>
    /// <returns>List of tower blueprints or null if none are found.</returns>
    public List<TowerBlueprint> GetAllTowers()
    {
        if (allTowers != null && allTowers.Count > 0) return allTowers;
        else
        {
            Debug.LogWarning("No towers found in inventory");
            return null;
        }
    }

    /// <summary>
    /// Creates and spawns a new tower using the blueprints in the inventory.
    /// </summary>
    private void SpawnTower()
    {
        GameObject tower = new GameObject("NewTower");
        foreach (TowerPart part in allTowers[0].allTowerParts)
        {
            GameObject createdPart = Instantiate<GameObject>(part.Module, part.Position, part.Rotation);
            createdPart.transform.SetParent(tower.transform);
        }
    }
}
