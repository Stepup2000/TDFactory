using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private BaseEnemy _parent;
    [SerializeField] private Slider _myHealthBar;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Gradient _colorGradient;


    private float _maxHealth;
    private float _currentHealth;
    private GameObject _mainCamera;
    private Quaternion originalRotation;

    private void OnEnable()
    {
        SetupHealthBar();
        if (_parent != null) _parent.OnHealthChanged += UpdateHealthbar;
    }

    private void OnDisable()
    {
        if (_parent != null) _parent.OnHealthChanged -= UpdateHealthbar;
    }

    //Makes sure the proper camera is found and the healthbar is added to the right canvas
    private void SetupHealthBar()
    {
        _mainCamera = GameObject.Find("Main Camera");
        originalRotation = transform.rotation;
        UpdateHealthbar(1, 1);
        TurnToCamera();
    }

    //Checks if the healthbar is properly setup, otherwise destroy itself
    private void ValidateHealthBar()
    {
        if (_mainCamera == null || _myHealthBar == null || _fillImage == null || _colorGradient == null)
        {
            Debug.LogWarning("Healthbar prefab was not set up properly");
            Destroy(gameObject);
        }
    }

    //Updates the healthbar based on maxvalue and currentvalue, uses a custom gradient to give a nice color effect
    public void UpdateHealthbar(float newValue, float maxValue)
    {
        ValidateHealthBar();

        _currentHealth = newValue;
        _maxHealth = maxValue;

        _myHealthBar.maxValue = _maxHealth;
        _myHealthBar.value = _currentHealth;
        _fillImage.color = _colorGradient.Evaluate(1 - _myHealthBar.value / _maxHealth);
    }

    //Makes the healthbar always face the given camera
    private void TurnToCamera()
    {
        if (_mainCamera)
        {
            transform.LookAt(_mainCamera.transform.position, Vector3.down);
            transform.rotation = _mainCamera.transform.rotation * originalRotation;
        }
    }
}

