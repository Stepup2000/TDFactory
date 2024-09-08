using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleButton : MonoBehaviour
{
    [SerializeField] private GameObject _modulePrefab; // The prefab for the module to be created
    [SerializeField] private DraggableModule _draggablePrefab; // The prefab for the draggable module

    /// <summary>
    /// Creates a new draggable module instance using the specified prefabs.
    /// </summary>
    public void CreateModuleDraggable()
    {
        if (_modulePrefab != null)
        {
            TowerBuilder.Instance.CreateNewDraggable(_modulePrefab, _draggablePrefab);
        }
    }
}
