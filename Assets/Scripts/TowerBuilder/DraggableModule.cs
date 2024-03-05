using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableModule : MonoBehaviour
{
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private Material _faultyPlacementMaterial;

    [SerializeField] private int _rotationAmount = 45;

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
            if (Input.GetMouseButton(0)) PlaceModule();
        }

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        float scrollAmount = Input.mouseScrollDelta.y;
        if (_createdModule != null)
        {
            if (scrollAmount > 0)
            {
                // Rotate the module counter-clockwise around its local Z-axis by 1 degree
                _createdModule.transform.Rotate(_createdModule.transform.up, -_rotationAmount);
            }
            else if (scrollAmount < 0)
            {
                // Rotate the module clockwise around its local Z-axis by 1 degree
                _createdModule.transform.Rotate(_createdModule.transform.up, _rotationAmount);
            }
        }
    }

    private void CalculatePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // If no layermask has been given, use the standard layermask of the TowerBodyLayerMask
        if (_targetMask == 0)
            _targetMask = 1 << 7;

        // Disable colliders in the created module to prevent raycast hits
        Collider[] moduleColliders = _createdModule.GetComponentsInChildren<Collider>();
        foreach (Collider collider in moduleColliders)
        {
            collider.enabled = false;
        }

        //Ray for determining canPlace
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _targetMask)) _canPlace = true;
        else _canPlace = false;

        //Ray for displaying the module in the right place
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 surfaceNormal = hit.normal;
            Vector3 moduleScale = _moduleCollider.transform.localScale;
            Vector3 moduleEdgePosition = hit.point + surfaceNormal * (moduleScale.y * 0.5f);

            // Check if the control key is pressed
            if (Input.GetKey(KeyCode.LeftControl))
            {
                // Round the moduleEdgePosition to the nearest 0.1 units
                moduleEdgePosition.x = Mathf.Round(moduleEdgePosition.x * 10) / 10;
                moduleEdgePosition.y = Mathf.Round(moduleEdgePosition.y * 10) / 10;
                moduleEdgePosition.z = Mathf.Round(moduleEdgePosition.z * 10) / 10;
            }

            _createdModule.transform.position = moduleEdgePosition;
        }

        // Enable colliders in the created module after raycast
        foreach (Collider collider in moduleColliders)
        {
            collider.enabled = true;
        }
    }




    //Returns the first collider that is not trigger
    private Collider GetCollider()
    {
        Collider foundCollider = null;
        Collider[] allcolliders = _createdModule.GetComponentsInChildren<Collider>();
        foreach (Collider collider in allcolliders)
        {
            if (!collider.isTrigger) foundCollider = collider;
        }
        return foundCollider;
    }

    //Create the module that will be placed
    private void CreateModule()
    {
        _createdModule = TowerBuilder.Instance.CreateModule(_modulePrefab);
        if (_createdModule != null)
        {
            _towerRenderer = _createdModule.GetComponentInChildren<MeshRenderer>();
            _towerOriginalMaterial = _towerRenderer.material;
        }        
    }

    //Actually place the created module
    private void PlaceModule()
    {
        if (_canPlace == true)
        {
            TowerBuilder.Instance.QueueModulePlacement(_createdModule);
            Destroy(gameObject);
        }
    }

    public void ClearDraggable()
    {
        if (_createdModule != null)
        {
            Destroy(_createdModule.gameObject);
            DestroyDraggable();
        }        
    }

    public void DestroyDraggable()
    {
        Destroy(gameObject);
    }

    public void SetModulePrefab(GameObject module)
    {
        _modulePrefab = module;
    }

    private void HandleMaterial()
    {
        if (_canPlace == true) SetOldMaterial();
        else SetTransparentMaterial();
    }

    private void SetOldMaterial()
    {
        if (_towerRenderer != null && _towerOriginalMaterial != null) _towerRenderer.material = _towerOriginalMaterial;
    }

    private void SetTransparentMaterial()
    {
        _towerRenderer.material = _faultyPlacementMaterial;
    }
}
