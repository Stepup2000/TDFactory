using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableTower : MonoBehaviour
{
    [SerializeField] private float _gridSize = 1f;
    private Tower _towerPrefab;

    // Update is called once per frame
    void FixedUpdate()
    {
        //Send raycast to know where the collisonCheck will be
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.CompareTag("Ground"))
        {
            Vector3 newPosition = hit.point;
            float newX = _gridSize * Mathf.RoundToInt(newPosition.x / _gridSize);
            float newY = 1;
            float newZ = _gridSize * Mathf.RoundToInt(newPosition.z / _gridSize);

            //Checks to see if the current position is not filled with another tower
            if (CheckForTowerCollision(newPosition)) return;
            transform.position = new Vector3(newX, newY, newZ);
            if (Input.GetMouseButton(0)) CreateTower();
        }
    }

    public void SetTowerPrefab(Tower prefab)
    {
        _towerPrefab = prefab;
    }

    // Create the tower and deduct money through the eventbus
    private void CreateTower()
    {
        if (_towerPrefab == null) return;
        EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(-_towerPrefab.GetStats(Tower.PRICE_STAT)));
        Instantiate<Tower>(_towerPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    //Checks to see if a tower is not at a position yet
    private bool CheckForTowerCollision(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapBox(position, transform.localScale / 2, Quaternion.identity);
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Tower>()) return true;
            }
        }
        return false;
    }
}
