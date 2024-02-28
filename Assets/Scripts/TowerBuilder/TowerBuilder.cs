using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] int _rotationAmount = 45;
    [SerializeField] float _rotationCooldown = 0.5f;
    [SerializeField] float _minimumMouseDragAmount = 0.001f;

    private static TowerBuilder instance;
    private List<TowerPart> _allTowerParts;
    private List<GameObject> _towerShowModel = new();
    private GameObject _towerParent;
    private DraggableModule _currentDraggableModule;
    private bool _canRotate = true;
    private Quaternion _oldTowerRotation;

    //Make sure there is only one instance
    public static TowerBuilder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TowerBuilder>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("TowerBuilder");
                    instance = singletonObject.AddComponent<TowerBuilder>();
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        _allTowerParts = new List<TowerPart>();
        _towerParent = new GameObject("TowerParent");
        _towerParent.transform.position = new Vector3(0, 1, 0);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1) && _canRotate) // Check if right mouse button is being held down and rotation is allowed
        {
            float mouseX = Input.GetAxis("Mouse X"); // Get horizontal movement of the mouse
            if (_towerShowModel != null)
            {
                if (mouseX > 0)
                {
                    // Rotate the module clockwise around its local Z-axis by 45 degrees
                    RotateTowerShowModel(-_rotationAmount);
                    StartCoroutine(WaitForRotation());
                }
                else if (mouseX < 0)
                {
                    // Rotate the module counter-clockwise around its local Z-axis by 45 degrees
                    RotateTowerShowModel(_rotationAmount);
                    StartCoroutine(WaitForRotation());
                }
            }
        }
    }

    private IEnumerator WaitForRotation()
    {
        _canRotate = false;
        yield return new WaitForSeconds(_rotationCooldown);
        _canRotate = true;
    }

    private void RotateTowerShowModel(int amount)
    {
        _towerParent.transform.Rotate(_towerParent.transform.up, amount);
    }

    private void ResetTowerShowModelRotation()
    {
        _oldTowerRotation = _towerParent.transform.rotation;
        _towerParent.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void SetTowerRotationToOld()
    {
        _towerParent.transform.rotation = _oldTowerRotation;
    }

    public void CreateNewDraggable(GameObject _modulePrefab, DraggableModule draggablePrefab)
    {
        ClearNewDragabble();

        DraggableModule newDragabble = Instantiate<DraggableModule>(draggablePrefab, transform.position, Quaternion.identity);
        newDragabble.SetModulePrefab(_modulePrefab);

        _currentDraggableModule = newDragabble;
        _currentDraggableModule.transform.SetParent(_towerParent.transform);
    }

    public void ClearNewDragabble()
    {
        if (_currentDraggableModule != null) _currentDraggableModule.ClearDraggable();
    }

    public GameObject CreateModule(GameObject modulePrefab)
    {
        if (modulePrefab == null)
        {
            Debug.LogError("Can't create module, module prefab is null.");
            return null;
        }

        return Instantiate(modulePrefab, transform.position, Quaternion.identity);
    }

    //Actually place the created module
    public void PlaceModule(GameObject moduleToPlace, GameObject modulePrefab)
    {
        moduleToPlace.transform.SetParent(_towerParent.transform);
        ResetTowerShowModelRotation();

        int cost = 0;
        Vector3 position = moduleToPlace.transform.position;
        Quaternion rotation = moduleToPlace.transform.rotation;

        moduleToPlace.TryGetComponent<IModule>(out IModule moduleComponent);

        if (moduleComponent != null) cost = moduleComponent.cost;

        TowerPart towerPartToAdd = new TowerPart(modulePrefab, cost, position, rotation);
        AddModuleToTower(towerPartToAdd, moduleToPlace);
        SetTowerRotationToOld();
    }

    public void AddModuleToTower(TowerPart towerPartToAdd, GameObject showModel)
    {
        _allTowerParts.Add(towerPartToAdd);
        _towerShowModel.Add(showModel);
    }

    public void RemoveModuleFromTower(TowerPart towerPartToRemove)
    {
        _allTowerParts.Remove(towerPartToRemove);
    }

    public List<TowerPart> GetAllTowerParts()
    {
        return _allTowerParts;
    }

    public void ConfirmTower()
    {
        ResetTowerShowModelRotation();

        int towerCost = 0;
        foreach (TowerPart towerPart in _allTowerParts)
        {
            towerCost += towerPart.moduleCost;
        }

        if (_allTowerParts != null && _allTowerParts.Count != 0 && towerCost != 0)
        {
            // Create a copy of _allTowerParts
            List<TowerPart> towerPartsCopy = new List<TowerPart>(_allTowerParts);

            TowerBlueprint newBlueprint = new TowerBlueprint(towerPartsCopy, towerCost);
            PlayerDataManager.Instance.AddTowerToInventory(newBlueprint);
            ClearWorkSpace();
        }
    }

    public void ClearWorkSpace()
    {
        foreach (GameObject model in _towerShowModel)
        {
            Destroy(model);
        }
        _allTowerParts.Clear();
    }
}
