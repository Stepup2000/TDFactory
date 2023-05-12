using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnInvisible : MonoBehaviour
{
    MeshRenderer _myRenderer;

    void Start()
    {
        TryGetComponent<MeshRenderer>(out _myRenderer);
        SetInvisibility(true);
    }

    public void SetInvisibility(bool trueOrFalse)
    {
        if (_myRenderer != null) _myRenderer.enabled = false;
    }
}
