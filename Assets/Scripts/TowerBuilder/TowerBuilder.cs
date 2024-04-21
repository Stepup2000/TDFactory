using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] int _rotationAmount = 90;
    [SerializeField] float _rotationCooldown = 0.5f;

    private static TowerBuilder instance;
    private List<TowerPart> _allTowerParts;
    private List<GameObject> _towerShowModel = new();
    private GameObject _towerParent;
    private DraggableModule _currentDraggableModule;
    private ModuleRemover _currentModuleRemover;
    private bool _canRotate = true;
    private Quaternion _oldTowerRotation;
    private int _currentTowerNumber = 0;

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
        CreateTowerParent();
        SetLoadbuttonsActive();
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

    private void CreateTowerParent()
    {
        if (_towerParent != null) Destroy(_towerParent.gameObject);
        _towerParent = new GameObject("TowerParent");
        _towerParent.transform.position = new Vector3(0, 1, 0);
    }

    private IEnumerator WaitForRotation()
    {
        _canRotate = false;
        yield return new WaitForSeconds(_rotationCooldown);
        _canRotate = true;
    }

    private void RotateTowerShowModel(int amount)
    {
        if (_towerParent != null) _towerParent.transform.Rotate(_towerParent.transform.up, amount);
    }

    private void ResetTowerShowModelRotation()
    {
        if (_towerParent != null)
        {
            _oldTowerRotation = _towerParent.transform.rotation;
            _towerParent.transform.rotation = Quaternion.Euler(0, 0, 0);
        }            
    }

    private void SetTowerRotationToOld()
    {
        _towerParent.transform.rotation = _oldTowerRotation;
    }

    public void CreateNewDraggable(GameObject _modulePrefab, DraggableModule draggablePrefab)
    {
        if (_modulePrefab != null && draggablePrefab != null)
        {
            if(_currentDraggableModule == null && _currentModuleRemover == null)
            {
                DraggableModule newDragabble = Instantiate<DraggableModule>(draggablePrefab, transform.position, Quaternion.identity);
                newDragabble.SetModulePrefab(_modulePrefab);

                _currentDraggableModule = newDragabble;
                _currentDraggableModule.transform.SetParent(_towerParent.transform);
            }
            else
            {
                ClearNewDragabble();
                ClearModuleRemover();
            }
        }        
    }

    public void CreateModuleRemover(ModuleRemover moduleRemover)
    {
        if (moduleRemover != null)
        {
            if (_currentDraggableModule == null && _currentModuleRemover == null)
            {
                ClearModuleRemover();
                ClearNewDragabble();

                ModuleRemover newModuleRemover = Instantiate<ModuleRemover>(moduleRemover, transform.position, Quaternion.identity);

                _currentModuleRemover = newModuleRemover;
            }
            else
            {
                ClearNewDragabble();
                ClearModuleRemover();
            }
        }
    }

    public void ClearNewDragabble()
    {
        if (_currentDraggableModule != null) _currentDraggableModule.ClearDraggable();
    }

    public void ClearModuleRemover()
    {
        if (_currentModuleRemover != null) Destroy(_currentModuleRemover.gameObject);
    }

    public GameObject CreateModule(GameObject modulePrefab)
    {
        if (modulePrefab == null)
        {
            Debug.LogError("Can't create module, module prefab is null.");
            return null;
        }

        GameObject newObject = Instantiate(modulePrefab, transform.position, Quaternion.identity);
        IModule newModule = newObject.GetComponentInChildren<IModule>();
        if (newModule != null) newModule.modulePrefab = modulePrefab;
        return newObject;
    }

    public void QueueModulePlacement(GameObject moduleToPlace)
    {
        moduleToPlace.transform.SetParent(_towerParent.transform);
    }

    //Actually place the created module
    public void PlaceModule(GameObject moduleToPlace, GameObject modulePrefab)
    {
        ResetTowerShowModelRotation();

        int cost = 0;
        Vector3 position = moduleToPlace.transform.position;
        Quaternion rotation = moduleToPlace.transform.rotation;

        IModule moduleComponent = moduleToPlace.GetComponentInChildren<IModule>();

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
        ClearModuleRemover();
        ClearNewDragabble();

        EventBus<RequestModuleDataEvent>.Publish(new RequestModuleDataEvent());
        ResetTowerShowModelRotation();


        int towerCost = 0;
        foreach (TowerPart towerPart in _allTowerParts)
        {
            towerCost += towerPart.moduleCost;
        }

        //Potentially add cost of 0 for the check
        if (_allTowerParts != null && _allTowerParts.Count != 0)
        {
            // Create a copy of _allTowerParts
            List<TowerPart> towerPartsCopy = new List<TowerPart>(_allTowerParts);

            TowerBlueprint newBlueprint = new TowerBlueprint(towerPartsCopy, towerCost);
            PlayerDataManager.Instance.TryRemoveTowerFromInventory(_currentTowerNumber);
            PlayerDataManager.Instance.AddTowerToInventory(newBlueprint, _currentTowerNumber);
        }
        ClearWorkSpace();
    }

    public void ClearWorkSpace()
    {
        CreateTowerParent();
        _allTowerParts = new List<TowerPart>();
    }

    public void LoadTower(int towerNumber)
    {
        _currentTowerNumber = towerNumber;
        List<TowerBlueprint> allTowers = PlayerDataManager.Instance.GetAllTowers();
        if (allTowers == null)
        {
            Debug.LogWarning("No towerlist was found");
            return;
        }
        else if (towerNumber < 0 || towerNumber >= allTowers.Count || allTowers[towerNumber] == null)
        {
            Debug.LogWarning("No saved tower found at index: " + towerNumber);
            return;
        }
        ClearWorkSpace();

        TowerBlueprint towerBlueprint = PlayerDataManager.Instance.GetAllTowers()[towerNumber];

        Tower towerModule = _towerParent.AddComponent<Tower>();
        Vector3 newPosition = new Vector3(transform.position.x, 0, transform.position.z);
        towerModule.transform.position = newPosition;

        foreach (TowerPart part in towerBlueprint.allTowerParts)
        {
            GameObject createdPart = Instantiate<GameObject>(part.module, newPosition + part.position, part.rotation);
            createdPart.transform.SetParent(_towerParent.transform);
            IModule module = createdPart.GetComponentInChildren<IModule>();
            if (module != null)
            {
                module.SetParentTower(towerModule);
                module.modulePrefab = part.module;
            }
        }
    }

    public void SetWeaponModulebuttonsActive()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Publish(new ToggleWeaponModuleButtonsEvent(true));
        EventBus<ToggleDetectionModuleButtonsEvent>.Publish(new ToggleDetectionModuleButtonsEvent(false));
        EventBus<ToggleBodyModuleButtonsEvent>.Publish(new ToggleBodyModuleButtonsEvent(false));
        EventBus<ToggleLoadButtonsEvent>.Publish(new ToggleLoadButtonsEvent(false));
    }

    public void SetDetectionModulebuttonsActive()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Publish(new ToggleWeaponModuleButtonsEvent(false));
        EventBus<ToggleDetectionModuleButtonsEvent>.Publish(new ToggleDetectionModuleButtonsEvent(true));
        EventBus<ToggleBodyModuleButtonsEvent>.Publish(new ToggleBodyModuleButtonsEvent(false));
        EventBus<ToggleLoadButtonsEvent>.Publish(new ToggleLoadButtonsEvent(false));
    }

    public void SetBodyModulebuttonsActive()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Publish(new ToggleWeaponModuleButtonsEvent(false));
        EventBus<ToggleDetectionModuleButtonsEvent>.Publish(new ToggleDetectionModuleButtonsEvent(false));
        EventBus<ToggleBodyModuleButtonsEvent>.Publish(new ToggleBodyModuleButtonsEvent(true));
        EventBus<ToggleLoadButtonsEvent>.Publish(new ToggleLoadButtonsEvent(false));
    }

    public void SetLoadbuttonsActive()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Publish(new ToggleWeaponModuleButtonsEvent(false));
        EventBus<ToggleDetectionModuleButtonsEvent>.Publish(new ToggleDetectionModuleButtonsEvent(false));
        EventBus<ToggleBodyModuleButtonsEvent>.Publish(new ToggleBodyModuleButtonsEvent(false));
        EventBus<ToggleLoadButtonsEvent>.Publish(new ToggleLoadButtonsEvent(true));
    }

    public void LoadLevel()
    {
        LevelManager.Instance.LoadLevel("SCE_Level");
    }
}
