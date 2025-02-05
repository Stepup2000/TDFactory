using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the behavior of a draggable module in the game. This includes 
/// creating, positioning, rotating, and placing the module based on user input.
/// </summary>
public class DraggableModule : MonoBehaviour
{
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private Material _faultyPlacementMaterial;

    [SerializeField] private int _rotationAmount = 90;

    private GameObject _modulePrefab;
    private GameObject _createdModule;
    private Collider _moduleCollider;
    private bool _canPlace = false;

    private MeshRenderer _towerRenderer;
    private Material _towerOriginalMaterial;
    public float transparency = 0.5f;

    private void Start()
    {
        CreateModule();
        _moduleCollider = GetCollider();
    }

    private void Update()
    {
        if (_modulePrefab != null && _createdModule != null && _moduleCollider != null)
        {
            CalculatePosition();
            HandleMaterial();
            if (Input.GetMouseButtonUp(0)) PlaceModule();
        }

        HandleMouseInput();
    }

    /// <summary>
    /// Handles mouse input for rotating the module.
    /// </summary>
    private void HandleMouseInput()
    {
        float scrollAmount = Input.mouseScrollDelta.y;
        if (_createdModule != null)
        {
            if (scrollAmount > 0)
            {
                // Rotate the module counter-clockwise around its local Z-axis by specified rotation amount
                _createdModule.transform.Rotate(_createdModule.transform.up, -_rotationAmount);
            }
            else if (scrollAmount < 0)
            {
                // Rotate the module clockwise around its local Z-axis by specified rotation amount
                _createdModule.transform.Rotate(_createdModule.transform.up, _rotationAmount);
            }
        }
    }

    /// <summary>
    /// Calculates and updates the position of the created module based on raycasting.
    /// </summary>
    private void CalculatePosition()
    {
        RaycastHit hit;
        Camera mainCamera = CameraController.Instance.GetCurrentCamera();
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // If no layer mask has been given, use a default layer mask
        if (_targetMask == 0)
            _targetMask = 1 << 7;

        // Disable colliders in the created module to prevent raycast hits
        Collider[] moduleColliders = _createdModule.GetComponentsInChildren<Collider>();
        foreach (Collider collider in moduleColliders)
        {
            collider.enabled = false;
        }

        int uiLayer = LayerMask.NameToLayer("UI");
        int uiLayerMask = 1 << uiLayer;

        // Raycast to check if any UI elements are hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, uiLayerMask))
        {
            _canPlace = false;
        }
        else
        {
            // No UI element hit, check against target mask
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _targetMask))
            {
                _canPlace = true;
            }
            else
            {
                _canPlace = false;
            }
        }

        // Update module position based on the surface hit by the ray
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 surfaceNormal = hit.normal;
            Vector3 moduleScale = _moduleCollider.transform.localScale;
            Vector3 moduleEdgePosition = hit.point + surfaceNormal * (moduleScale.y * 0.5f);

            // Round the module edge position to the nearest unit
            moduleEdgePosition.x = Mathf.Round(moduleEdgePosition.x * 1) / 1;
            moduleEdgePosition.y = Mathf.Round(moduleEdgePosition.y * 1) / 1;
            moduleEdgePosition.z = Mathf.Round(moduleEdgePosition.z * 1) / 1;

            _createdModule.transform.position = moduleEdgePosition;
        }

        // Enable colliders in the created module after raycasting
        foreach (Collider collider in moduleColliders)
        {
            collider.enabled = true;
        }
    }

    /// <summary>
    /// Returns the first non-trigger collider found in the created module.
    /// </summary>
    /// <returns>The first non-trigger collider.</returns>
    private Collider GetCollider()
    {
        Collider foundCollider = null;
        Collider[] allColliders = _createdModule.GetComponentsInChildren<Collider>();
        foreach (Collider collider in allColliders)
        {
            if (!collider.isTrigger) foundCollider = collider;
        }
        return foundCollider;
    }

    /// <summary>
    /// Creates an instance of the module prefab and initializes its renderer and material.
    /// </summary>
    private void CreateModule()
    {
        _createdModule = TowerBuilder.Instance.CreateModule(_modulePrefab);
        if (_createdModule != null)
        {
            _towerRenderer = _createdModule.GetComponentInChildren<MeshRenderer>();
            _towerOriginalMaterial = _towerRenderer.material;
        }
    }

    /// <summary>
    /// Places the created module if it can be placed, plays a placement sound, and creates a new module.
    /// </summary>
    private void PlaceModule()
    {
        if (_canPlace)
        {
            TowerBuilder.Instance.QueueModulePlacement(_createdModule);
            CreateModule();
            AudioClip clip = _createdModule.GetComponentInChildren<IModule>().placementSoundClip;
            SoundManager.Instance.PlaySoundAtLocation(clip, _createdModule.transform.position, true);
        }
    }

    /// <summary>
    /// Clears the currently created module and destroys the draggable module.
    /// </summary>
    public void ClearDraggable()
    {
        if (_createdModule != null)
        {
            Destroy(_createdModule.gameObject);
            DestroyDraggable();
        }
    }

    /// <summary>
    /// Destroys the draggable module game object.
    /// </summary>
    public void DestroyDraggable()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Sets the module prefab to be used for creating the module.
    /// </summary>
    /// <param name="module">The module prefab to set.</param>
    public void SetModulePrefab(GameObject module)
    {
        _modulePrefab = module;
    }

    /// <summary>
    /// Updates the material of the created module based on whether it can be placed or not.
    /// </summary>
    private void HandleMaterial()
    {
        if (_canPlace) SetOldMaterial();
        else SetTransparentMaterial();
    }

    /// <summary>
    /// Sets the material of the module to its original material.
    /// </summary>
    private void SetOldMaterial()
    {
        if (_towerRenderer != null && _towerOriginalMaterial != null)
            _towerRenderer.material = _towerOriginalMaterial;
    }

    /// <summary>
    /// Sets the material of the module to indicate an invalid placement.
    /// </summary>
    private void SetTransparentMaterial()
    {
        _towerRenderer.material = _faultyPlacementMaterial;
    }
}
