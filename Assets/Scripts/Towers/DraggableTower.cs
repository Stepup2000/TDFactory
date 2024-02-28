using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableTower : MonoBehaviour
{
    [SerializeField] private int _rotationAmount = 45;

    private Tower _towerPrefab;
    private Tower _createdTower;
    private bool _canPlace = false;

    private void Start()
    {
        TowerBlueprint towerBlueprint = PlayerDataManager.Instance.GetAllTowers()[0];
        SpawnTower(towerBlueprint);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_towerPrefab != null && _createdTower != null)
        {
            // Send raycast to know where the collision check will be
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 6;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                _createdTower.transform.position = hit.point;
                _canPlace = true;
            }
            else _canPlace = false;
            if (Input.GetMouseButton(0)) ActivateTower();
        }
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        float scrollAmount = Input.mouseScrollDelta.y;
        if (_createdTower != null)
        {
            if (scrollAmount > 0)
            {
                // Rotate the module counter-clockwise around its local Z-axis by 1 degree
                _createdTower.transform.Rotate(_createdTower.transform.up, -_rotationAmount);
            }
            else if (scrollAmount < 0)
            {
                // Rotate the module clockwise around its local Z-axis by 1 degree
                _createdTower.transform.Rotate(_createdTower.transform.up, _rotationAmount);
            }
        }
    }

    public void SetTowerPrefab(Tower prefab)
    {
        _towerPrefab = prefab;
    }

    // Create the tower and deduct money through the eventbus
    private void SpawnTower(TowerBlueprint towerBlueprint)
    {
        GameObject tower = new GameObject("NewTower");
        _createdTower = tower.AddComponent<Tower>();
        Vector3 newPosition = new Vector3(transform.position.x, 0, transform.position.z);
        _createdTower.transform.position = newPosition;

        //_createdTower = Instantiate<Tower>(_towerPrefab, newPosition, Quaternion.identity);
        
        foreach (TowerPart part in towerBlueprint.allTowerParts)
        {
            GameObject createdPart = Instantiate<GameObject>(part.module, newPosition + part.position, part.rotation);
            createdPart.transform.SetParent(tower.transform);
            IModule module = createdPart.GetComponentInChildren<IModule>();
            if (module != null) module.SetParentTower(_createdTower);
        }
    }

    private void ActivateTower()
    {
        if (_createdTower != null && _canPlace && true)
        {
            EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(-_towerPrefab.GetStats(Tower.PRICE_STAT)));
            _createdTower.ActivateTower();
            Destroy(gameObject);
        }        
    }
}
