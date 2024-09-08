using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the removal of modules based on user input and placement calculations.
/// </summary>
public class ModuleRemover : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask; // Layer mask to filter target layers for raycasting.

    [SerializeField] private float destructionCooldown = 0.1f; // Cooldown time before allowing the next destruction.

    [SerializeField] private AudioClip removeSoundClip; // Sound clip to play when a module is removed.

    private bool objectDestroyed = false; // Flag to track if an object has been destroyed.

    private void Update()
    {
        CalculatePosition(); // Calculate and update the position of the module.
        if (Input.GetMouseButtonUp(0))
        {
            TryRemoveModule(); // Try to remove the module if the left mouse button is released.
        }
    }

    /// <summary>
    /// Calculates and updates the position of the module based on raycasting.
    /// </summary>
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

        // Ray for displaying the module in the right place
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetMask))
        {
            // Move to the position of the object it hit
            transform.position = hit.collider.transform.position;
        }
        else
        {
            // Ray for displaying the module in the right place
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 surfaceNormal = hit.normal;
                Vector3 moduleScale = transform.localScale;
                Vector3 moduleEdgePosition = hit.point + surfaceNormal * (moduleScale.y * 0.5f);

                // Round the moduleEdgePosition to the nearest 1 unit
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

    /// <summary>
    /// Attempts to remove a module by checking for colliders in the vicinity.
    /// </summary>
    private void TryRemoveModule()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 10, transform.rotation);
        foreach (Collider collider in colliders)
        {
            if (!objectDestroyed) // Check if an object has already been destroyed
            {
                CheckForModuleParent(collider.gameObject);
            }
            else
            {
                break; // If an object has been destroyed, stop checking
            }
        }
    }

    /// <summary>
    /// Recursively checks for a module parent in the hierarchy and removes it if found.
    /// </summary>
    /// <param name="testedObject">The game object to check for module parent status.</param>
    private void CheckForModuleParent(GameObject testedObject)
    {
        if (IsModuleParent(testedObject))
        {
            SoundManager.Instance.PlaySoundAtLocation(removeSoundClip, transform.position, true);
            Destroy(testedObject.gameObject);
            ToggleObjectDestroyed();
        }
        else if (testedObject.transform.parent != null)
        {
            CheckForModuleParent(testedObject.transform.parent.gameObject);
        }
    }

    /// <summary>
    /// Checks if the provided game object is a module parent.
    /// </summary>
    /// <param name="testedObject">The game object to check.</param>
    /// <returns>True if the game object is a module parent; otherwise, false.</returns>
    private bool IsModuleParent(GameObject testedObject)
    {
        return testedObject.layer == LayerMask.NameToLayer("ModuleParent");
    }

    /// <summary>
    /// Toggles the object destroyed flag and sets up a recurring call to itself after the destruction cooldown.
    /// </summary>
    private void ToggleObjectDestroyed()
    {
        objectDestroyed = !objectDestroyed;
        Invoke("ToggleObjectDestroyed", destructionCooldown);
    }
}
