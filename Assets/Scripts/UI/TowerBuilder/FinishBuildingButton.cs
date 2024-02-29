using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishBuildingButton : MonoBehaviour
{
    public void OnClick()
    {
        TowerBuilder.Instance.ConfirmTower();
        LevelManager.Instance.LoadLevel("Level");
    }
}
