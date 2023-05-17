using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableTower : MonoBehaviour
{
    private Tower _towerPrefab;
    private Tower createdTower;
    private bool _canPlace = false;

    private void Start()
    {
        SpawnTower();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (createdTower != null)
        {
            //Send raycast to know where the collisonCheck will be
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.CompareTag("Ground"))
            {
                _canPlace = true;
                createdTower.transform.position = hit.point;
            }
            if (Input.GetMouseButton(0)) ActivateTower();
        }        
    }

    public void SetTowerPrefab(Tower prefab)
    {
        _towerPrefab = prefab;
    }

    // Create the tower and deduct money through the eventbus
    private void SpawnTower()
    {
        Vector3 newPosition = new Vector3(transform.position.x, 0, transform.position.z);
        createdTower = Instantiate<Tower>(_towerPrefab, newPosition, Quaternion.identity);
    }

    private void ActivateTower()
    {
        if (createdTower != null && _canPlace && true)
        {
            Debug.Log(_towerPrefab.GetStats(Tower.PRICE_STAT));
            EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(-_towerPrefab.GetStats(Tower.PRICE_STAT)));
            createdTower.ActivateTower();
            Destroy(gameObject);
        }        
    }
}
