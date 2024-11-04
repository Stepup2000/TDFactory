using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the construction and management of towers in the game.
/// Includes functionality for rotating towers, creating draggable modules, placing modules, and saving/loading tower configurations.
/// </summary>
public class TowerBuilder : MonoBehaviour
{
    [SerializeField] int _rotationAmount = 90;
    [SerializeField] float _rotationCooldown = 0.5f;
    [SerializeField] PlayButtonActivator playButtonActivator;

    private static TowerBuilder instance;
    private List<TowerPart> _allTowerParts;
    private List<GameObject> _towerShowModel = new();
    private GameObject _towerParent;
    private DraggableModule _currentDraggableModule;
    private ModuleRemover _currentModuleRemover;
    private bool _canRotate = true;
    private Quaternion _oldTowerRotation;
    private int _currentTowerNumber = 0;
    private GameObject _previousModulePrefab;

    /// <summary>
    /// Singleton instance of the TowerBuilder.
    /// Ensures there is only one instance in the scene.
    /// </summary>
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
        _allTowerParts = new List<TowerPart>(); // Initialize list of tower parts
        CreateTowerParent(); // Create parent object for the tower
        SetLoadbuttonsActive(); // Activate load buttons in the UI
    }

    private void Update()
    {
        // Rotate the tower when right mouse button is pressed and rotation is allowed
        if (Input.GetMouseButton(1) && _canRotate)
        {
            float mouseX = Input.GetAxis("Mouse X"); // Get horizontal mouse movement

            if (_towerShowModel != null)
            {
                if (mouseX > 0) // Rotate clockwise
                {
                    RotateTowerShowModel(-_rotationAmount);
                    StartCoroutine(WaitForRotation()); // Wait for cooldown
                }
                else if (mouseX < 0) // Rotate counter-clockwise
                {
                    RotateTowerShowModel(_rotationAmount);
                    StartCoroutine(WaitForRotation()); // Wait for cooldown
                }
            }
        }
    }

    /// <summary>
    /// Creates the parent GameObject for the tower parts.
    /// Destroys any previous parent object to ensure there is only one.
    /// </summary>
    private void CreateTowerParent()
    {
        if (_towerParent != null) Destroy(_towerParent.gameObject);
        _towerParent = new GameObject("TowerParent");
        _towerParent.transform.position = new Vector3(0, 1, 0);
    }

    /// <summary>
    /// Waits for a cooldown period before allowing further rotations.
    /// </summary>
    private IEnumerator WaitForRotation()
    {
        _canRotate = false;
        yield return new WaitForSeconds(_rotationCooldown);
        _canRotate = true;
    }

    /// <summary>
    /// Rotates the tower model by a given amount.
    /// </summary>
    /// <param name="amount">Amount of rotation in degrees.</param>
    private void RotateTowerShowModel(int amount)
    {
        if (_towerParent != null) _towerParent.transform.Rotate(_towerParent.transform.up, amount);
    }

    /// <summary>
    /// Resets the rotation of the tower model to its default rotation (0, 0, 0).
    /// </summary>
    private void ResetTowerShowModelRotation()
    {
        if (_towerParent != null)
        {
            _oldTowerRotation = _towerParent.transform.rotation;
            _towerParent.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    /// <summary>
    /// Restores the tower's rotation to its previous state before resetting.
    /// </summary>
    private void SetTowerRotationToOld()
    {
        _towerParent.transform.rotation = _oldTowerRotation;
    }

    /// <summary>
    /// Creates a new draggable module for the tower.
    /// </summary>
    /// <param name="_modulePrefab">The prefab for the module to be created.</param>
    /// <param name="draggablePrefab">The draggable module prefab.</param>
    public void CreateNewDraggable(GameObject _modulePrefab, DraggableModule draggablePrefab)
    {
        //Clear the draggables, and only make a new one if its a new button
        if (_modulePrefab != null && draggablePrefab != null)
        {
            ClearNewDraggable();
            ClearModuleRemover();
            if (_modulePrefab != _previousModulePrefab)            
            {
                DraggableModule newDraggable = Instantiate<DraggableModule>(draggablePrefab, transform.position, Quaternion.identity);
                newDraggable.SetModulePrefab(_modulePrefab);

                _currentDraggableModule = newDraggable;
                _currentDraggableModule.transform.SetParent(_towerParent.transform);
                _previousModulePrefab = _modulePrefab;
            }
            else
            {
                //Reset previous module prefab (since the button is pressed a second time it should cancel placement)
                _previousModulePrefab = null;
            }
        }
    }

    /// <summary>
    /// Creates a module remover for removing modules from the tower.
    /// </summary>
    /// <param name="moduleRemover">The module remover prefab.</param>
    public void CreateModuleRemover(ModuleRemover moduleRemover)
    {
        if (moduleRemover != null)
        {
            if (_currentDraggableModule == null && _currentModuleRemover == null)
            {
                ClearModuleRemover();
                ClearNewDraggable();

                ModuleRemover newModuleRemover = Instantiate<ModuleRemover>(moduleRemover, transform.position, Quaternion.identity);

                _currentModuleRemover = newModuleRemover;
            }
            else
            {
                ClearNewDraggable();
                ClearModuleRemover();
            }
        }
    }

    /// <summary>
    /// Clears the currently active draggable module.
    /// </summary>
    public void ClearNewDraggable()
    {
        if (_currentDraggableModule != null) _currentDraggableModule.ClearDraggable();
    }

    /// <summary>
    /// Clears the currently active module remover.
    /// </summary>
    public void ClearModuleRemover()
    {
        if (_currentModuleRemover != null) Destroy(_currentModuleRemover.gameObject);
    }

    /// <summary>
    /// Instantiates a new module in the scene.
    /// </summary>
    /// <param name="modulePrefab">The prefab of the module to be created.</param>
    /// <returns>The newly created module GameObject.</returns>
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

    /// <summary>
    /// Queues the placement of a module by parenting it to the tower.
    /// </summary>
    /// <param name="moduleToPlace">The module to be placed.</param>
    public void QueueModulePlacement(GameObject moduleToPlace)
    {
        moduleToPlace.transform.SetParent(_towerParent.transform);
    }

    /// <summary>
    /// Places a module onto the tower.
    /// </summary>
    /// <param name="moduleToPlace">The module to be placed.</param>
    /// <param name="modulePrefab">The prefab for the module.</param>
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

    /// <summary>
    /// Adds a module to the tower and updates the tower's model list.
    /// </summary>
    /// <param name="towerPartToAdd">The tower part to be added.</param>
    /// <param name="showModel">The visual model of the module.</param>
    public void AddModuleToTower(TowerPart towerPartToAdd, GameObject showModel)
    {
        _allTowerParts.Add(towerPartToAdd);
        _towerShowModel.Add(showModel);
    }

    /// <summary>
    /// Removes a module from the tower.
    /// </summary>
    /// <param name="towerPartToRemove">The tower part to be removed.</param>
    public void RemoveModuleFromTower(TowerPart towerPartToRemove)
    {
        _allTowerParts.Remove(towerPartToRemove);
    }

    /// <summary>
    /// Gets the list of all parts that make up the tower.
    /// </summary>
    /// <returns>List of tower parts.</returns>
    public List<TowerPart> GetAllTowerParts()
    {
        return _allTowerParts;
    }

    /// <summary>
    /// Confirms the tower's configuration, calculating its cost and saving it.
    /// </summary>
    public void ConfirmTower()
    {
        ClearModuleRemover();
        ClearNewDraggable();

        EventBus<RequestModuleDataEvent>.Publish(new RequestModuleDataEvent());
        ResetTowerShowModelRotation();

        int towerCost = 0;
        foreach (TowerPart towerPart in _allTowerParts)
        {
            towerCost += towerPart.ModuleCost;
        }

        if (_allTowerParts != null && _allTowerParts.Count != 0)
        {
            List<TowerPart> towerPartsCopy = new List<TowerPart>(_allTowerParts);

            TowerBlueprint newBlueprint = new TowerBlueprint(towerPartsCopy, towerCost);
            PlayerDataManager.Instance.TryRemoveTowerFromInventory(_currentTowerNumber);
            PlayerDataManager.Instance.AddTowerToInventory(newBlueprint, _currentTowerNumber);
        }
        ClearWorkSpace();
        playButtonActivator?.CheckForBlueprint();
    }

    /// <summary>
    /// Clears the current workspace, resetting the tower's parent and parts list.
    /// </summary>
    public void ClearWorkSpace()
    {
        CreateTowerParent();
        _allTowerParts = new List<TowerPart>();
    }

    /// <summary>
    /// Loads a saved tower configuration by its index.
    /// </summary>
    /// <param name="towerNumber">The index of the tower to load.</param>
    public void LoadTower(int towerNumber)
    {
        _currentTowerNumber = towerNumber;
        List<TowerBlueprint> allTowers = PlayerDataManager.Instance.GetAllTowers();
        if (allTowers == null)
        {
            Debug.LogWarning("No tower list was found");
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
            GameObject createdPart = Instantiate<GameObject>(part.Module, newPosition + part.Position, part.Rotation);
            createdPart.transform.SetParent(_towerParent.transform);
            IModule module = createdPart.GetComponentInChildren<IModule>();
            if (module != null)
            {
                module.SetParentTower(towerModule);
                module.modulePrefab = part.Module;
            }
        }
    }

    /// <summary>
    /// Activates the buttons for selecting weapon modules and deactivates others.
    /// </summary>
    public void SetWeaponModulebuttonsActive()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Publish(new ToggleWeaponModuleButtonsEvent(true)); // Enable weapon module buttons
        EventBus<ToggleDetectionModuleButtonsEvent>.Publish(new ToggleDetectionModuleButtonsEvent(false)); // Disable detection module buttons
        EventBus<ToggleBodyModuleButtonsEvent>.Publish(new ToggleBodyModuleButtonsEvent(false)); // Disable body module buttons
        EventBus<ToggleLoadButtonsEvent>.Publish(new ToggleLoadButtonsEvent(false)); // Disable load buttons
    }

    /// <summary>
    /// Activates the buttons for selecting detection modules and deactivates others.
    /// </summary>
    public void SetDetectionModulebuttonsActive()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Publish(new ToggleWeaponModuleButtonsEvent(false)); // Disable weapon module buttons
        EventBus<ToggleDetectionModuleButtonsEvent>.Publish(new ToggleDetectionModuleButtonsEvent(true)); // Enable detection module buttons
        EventBus<ToggleBodyModuleButtonsEvent>.Publish(new ToggleBodyModuleButtonsEvent(false)); // Disable body module buttons
        EventBus<ToggleLoadButtonsEvent>.Publish(new ToggleLoadButtonsEvent(false)); // Disable load buttons
    }

    /// <summary>
    /// Activates the buttons for selecting body modules and deactivates others.
    /// </summary>
    public void SetBodyModulebuttonsActive()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Publish(new ToggleWeaponModuleButtonsEvent(false)); // Disable weapon module buttons
        EventBus<ToggleDetectionModuleButtonsEvent>.Publish(new ToggleDetectionModuleButtonsEvent(false)); // Disable detection module buttons
        EventBus<ToggleBodyModuleButtonsEvent>.Publish(new ToggleBodyModuleButtonsEvent(true)); // Enable body module buttons
        EventBus<ToggleLoadButtonsEvent>.Publish(new ToggleLoadButtonsEvent(false)); // Disable load buttons
    }

    /// <summary>
    /// Activates the load buttons and deactivates others.
    /// </summary>
    public void SetLoadbuttonsActive()
    {
        EventBus<ToggleWeaponModuleButtonsEvent>.Publish(new ToggleWeaponModuleButtonsEvent(false)); // Disable weapon module buttons
        EventBus<ToggleDetectionModuleButtonsEvent>.Publish(new ToggleDetectionModuleButtonsEvent(false)); // Disable detection module buttons
        EventBus<ToggleBodyModuleButtonsEvent>.Publish(new ToggleBodyModuleButtonsEvent(false)); // Disable body module buttons
        EventBus<ToggleLoadButtonsEvent>.Publish(new ToggleLoadButtonsEvent(true)); // Enable load buttons
    }


    /// <summary>
    /// Loads the game level where tower defense occurs.
    /// </summary>
    public void LoadLevel()
    {
        LevelManager.LoadLevel("Level");
    }
}
