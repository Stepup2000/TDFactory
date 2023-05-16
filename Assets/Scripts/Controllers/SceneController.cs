using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField] LevelData _levelData;
    private static SceneController instance;

    //Make sure there is only one instance
    public static SceneController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneController>();
                if (instance == null)
                {
                    GameObject singletonObject = new();
                    instance = singletonObject.AddComponent<SceneController>();
                    singletonObject.name = new string("SceneManager");
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Start()
    {
        Invoke("InitializeLevel", 0.1f);
    }

    private void InitializeLevel()
    {
        if (_levelData != null)
        {
            EventBus<InitializeLevel>.Publish(new InitializeLevel(_levelData));
            EventBus<ChangeMoneyEvent>.Publish(new ChangeMoneyEvent(_levelData.StartingCurrency));
            EventBus<ChangeHealthEvent>.Publish(new ChangeHealthEvent(_levelData.StartingHealth));
        }        
    }
}
