using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleRemover : MonoBehaviour
{
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private float _destructionCooldown = 0.1f;
    [SerializeField] AudioClip _removeSoundClip;
    private bool objectDestroyed = false;

    private void Update()
    {
        CalculatePosition();
        if (Input.GetMouseButtonUp(0)) TryRemoveModule();
    }

    private void CalculatePosition()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Disable colliders in the created module to prevent raycast hits
        Collider[] moduleColliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in moduleColliders)
        {
            collider.enabled = false;
        }

        //Ray for displaying the module in the right place
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _targetMask))
        {
            // Move to the position of the object it hit
            transform.position = hit.collider.transform.position;
        }
        else
        {
            //Ray for displaying the module in the right place
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 surfaceNormal = hit.normal;
                Vector3 moduleScale = transform.localScale;
                Vector3 moduleEdgePosition = hit.point + surfaceNormal * (moduleScale.y * 0.5f);

                // Round the moduleEdgePosition to the nearest 1 units
                moduleEdgePosition.x = Mathf.Round(moduleEdgePosition.x * 1) / 1;
                moduleEdgePosition.y = Mathf.Round(moduleEdgePosition.y * 1) / 1;
                moduleEdgePosition.z = Mathf.Round(moduleEdgePosition.z * 1) / 1;

                transform.position = moduleEdgePosition;
            }
        }

        // Enable colliders in the created module after raycast
        foreach (Collider collider in moduleColliders)
        {
            collider.enabled = true;
        }
    }


    private void TryRemoveModule()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 10, transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (!objectDestroyed) // Check if an object has already been destroyed
                CheckForModuleParent(collider.gameObject);
            else
                break; // If an object has been destroyed, stop checking
        }
    }

    private void CheckForModuleParent(GameObject testedObject)
    {
        if (IsModuleParent(testedObject))
        {
            SoundManager.Instance.PlaySoundAtLocation(_removeSoundClip, transform.position, true);
            Destroy(testedObject.gameObject);
            ToggleObjectDestroyed();
        }
        else if (testedObject.transform.parent != null)
        {
            CheckForModuleParent(testedObject.transform.parent.gameObject);
        }
    }

    private bool IsModuleParent(GameObject testedObject)
    {
        return testedObject.layer == LayerMask.NameToLayer("ModuleParent");
    }

    private void ToggleObjectDestroyed()
    {
        objectDestroyed = !objectDestroyed;
        Invoke("ToggleObjectDestroyed", _destructionCooldown);
    }
}
