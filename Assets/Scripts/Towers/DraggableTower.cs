using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableTower : MonoBehaviour
{
    /// <summary>
    /// The amount by which the tower rotates when the mouse scrolls.
    /// </summary>
    [SerializeField] private int _rotationAmount = 45;

    private Tower _createdTower; // Reference to the currently created tower.
    private bool _canPlace = false; // Indicates whether the tower can be placed at the current location.

    /// <summary>
    /// Initializes the tower by spawning it using the player's first available tower blueprint.
    /// </summary>
    private void Start()
    {
        
    }

    /// <summary>
    /// Sets the blueprint for the tower prefab.
    /// </summary>
    public void SetTowerBlueprint(int towerNumber)
    {
        TowerBlueprint towerBlueprint = PlayerDataManager.Instance.GetAllTowers()[towerNumber];
        SpawnTower(towerBlueprint);
    }

    /// <summary>
    /// Updates the position of the tower being dragged and checks if it can be placed at the current location.
    /// </summary>
    private void FixedUpdate()
    {
        if (_createdTower != null)
        {
            RaycastHit hit;
            Camera mainCamera = CameraController.Instance.GetCurrentCamera();
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 6; // Only interact with the specified layer.

            // Cast a ray to determine where the tower should be positioned.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                _createdTower.transform.position = hit.point; // Move the tower to the raycast hit position.
                _canPlace = true; // Tower can be placed.
            }
            else _canPlace = false;

            // If left mouse button is clicked, activate the tower.
            if (Input.GetMouseButton(0)) ActivateTower();
        }
    }

    /// <summary>
    /// Handles mouse input for rotating the tower.
    /// </summary>
    private void Update()
    {
        HandleMouseInput();
    }

    /// <summary>
    /// Rotates the tower based on the mouse scroll input.
    /// </summary>
    private void HandleMouseInput()
    {
        float scrollAmount = Input.mouseScrollDelta.y;
        if (_createdTower != null)
        {
            if (scrollAmount > 0)
            {
                _createdTower.transform.Rotate(_createdTower.transform.up, -_rotationAmount); // Rotate counter-clockwise.
            }
            else if (scrollAmount < 0)
            {
                _createdTower.transform.Rotate(_createdTower.transform.up, _rotationAmount); // Rotate clockwise.
            }
        }
    }

    /// <summary>
    /// Spawns the tower using the provided tower blueprint and attaches the necessary parts.
    /// </summary>
    /// <param name="towerBlueprint">The blueprint that contains information about the tower's structure and modules.</param>
    private void SpawnTower(TowerBlueprint towerBlueprint)
    {
        GameObject tower = new GameObject("NewTower"); // Create a new GameObject for the tower.
        _createdTower = tower.AddComponent<Tower>(); // Attach a Tower component to the GameObject.

        Vector3 newPosition = new Vector3(transform.position.x, 0, transform.position.z);
        _createdTower.transform.position = newPosition; // Set the initial position of the tower.

        // Instantiate and attach each tower part based on the blueprint.
        foreach (TowerPart part in towerBlueprint.allTowerParts)
        {
            GameObject createdPart = Instantiate(part.Module, newPosition + part.Position, part.Rotation);
            createdPart.transform.SetParent(tower.transform); // Attach the part to the tower as a child.
            IModule module = createdPart.GetComponentInChildren<IModule>(); // Get the module component if available.
            if (module != null) module.SetParentTower(_createdTower); // Assign the parent tower to the module.
        }
    }

    /// <summary>
    /// Activates the tower if it can be placed, switching the camera back to the main view and destroying the draggable object.
    /// </summary>
    private void ActivateTower()
    {
        if (_createdTower != null && _canPlace)
        {
            _createdTower.ActivateTower(); // Activate the tower's functionality.
            CameraController.Instance.ChangeCamera("MainCamera"); // Switch back to the main camera.
            Destroy(gameObject); // Destroy the draggable object.
        }
    }
}
