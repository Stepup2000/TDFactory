using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
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

    public void AddTowerToInventory(TowerBlueprint tower)
    {
        allTowers ??= new List<TowerBlueprint>();
        allTowers.Add(tower);
    }

    public void RemoveTowerFromInventory(TowerBlueprint tower)
    {
        if (allTowers != null && allTowers.Contains(tower))
        {
            allTowers.Remove(tower);
        }
        else
        {
            Debug.LogWarning("Tower not found in inventory");
        }
    }

    public List<TowerBlueprint> GetAllTowers()
    {
        if (allTowers != null) return allTowers;
        else
        {
            Debug.LogWarning("NoTowersFoundInInventory");
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