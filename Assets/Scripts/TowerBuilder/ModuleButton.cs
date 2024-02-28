using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleButton : MonoBehaviour
{
    [SerializeField] private GameObject _modulePrefab;
    [SerializeField] private DraggableModule _draggablePrefab;

    public void CreateModuleDraggable()
    {
        if (_modulePrefab != null) TowerBuilder.Instance.CreateNewDraggable(_modulePrefab, _draggablePrefab);
    }
}
