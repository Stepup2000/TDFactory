using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private int _amountOfSaveslots = 3;
    private List<TowerBlueprint> allTowers = new List<TowerBlueprint>();
    private static PlayerDataManager instance;

    //Make sure there is only one instance
    public static PlayerDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerDataManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("playerDataManager");
                    instance = singletonObject.AddComponent<PlayerDataManager>();
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void AddTowerToInventory(TowerBlueprint tower, int towerNumber)
    {
        Debug.Log(tower.allTowerParts.Count);
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

        // Create a new instance of TowerBlueprint for the tower being added
        //TowerBlueprint newTower = new TowerBlueprint();
        //newTower.allTowerParts = new List<TowerPart>(tower.allTowerParts); // Optionally copy tower parts if needed
        // Add the tower at the specified towerNumber location
        allTowers[towerNumber] = tower;
    }

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

    public List<TowerBlueprint> GetAllTowers()
    {
        if (allTowers != null) return allTowers;
        else
        {
            Debug.LogWarning("No towers found in inventory");
            return null;
        }        
    }

    private void SpawnTower()
    {
        GameObject tower = new GameObject("NewTower");
        foreach (TowerPart part in allTowers[0].allTowerParts)
        {
            GameObject createdPart = Instantiate<GameObject>(part.module, part.position, part.rotation);
            createdPart.transform.SetParent(tower.transform);
        }        
    }
}