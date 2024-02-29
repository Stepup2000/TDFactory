using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelPlacingButton : MonoBehaviour
{
    public void OnClick()
    {
        TowerBuilder.Instance.ClearNewDragabble();
    }
}
