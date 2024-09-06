using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the invisibility of the game object by enabling or disabling its MeshRenderer component.
/// </summary>
public class TurnInvisible : MonoBehaviour
{
    private MeshRenderer _myRenderer;

    /// <summary>
    /// Initializes the script by finding the MeshRenderer component and setting the initial invisibility state.
    /// </summary>
    void Start()
    {
        TryGetComponent(out _myRenderer);
        SetInvisibility(true); // Initial state
    }

    /// <summary>
    /// Sets the invisibility of the game object by enabling or disabling its MeshRenderer.
    /// </summary>
    /// <param name="trueOrFalse">If true, the game object becomes invisible; if false, it becomes visible.</param>
    public void SetInvisibility(bool trueOrFalse)
    {
        if (_myRenderer != null)
        {
            _myRenderer.enabled = !trueOrFalse; // Set renderer active based on the parameter
        }
    }
}
