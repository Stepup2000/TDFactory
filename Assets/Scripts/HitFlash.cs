using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private Material _hitMaterial;
    [SerializeField] private float _flashDuration = 0.05f;
    private Material[] _originalMaterials;
    private Renderer[] _renderers;
    private BaseEnemy _targetEnemy;

    private void OnEnable()
    {
        if (TryGetComponent<BaseEnemy>(out _targetEnemy)) _targetEnemy.OnHealthChanged += TriggerHitEffect;
    }

    private void OnDisable()
    {
        if (_targetEnemy != null) _targetEnemy.OnHealthChanged -= TriggerHitEffect;
    }

    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalMaterials = new Material[_renderers.Length];

        // Store the original material for each renderer
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalMaterials[i] = _renderers[i].material;
        }
    }

    public void TriggerHitEffect(float newValue, float maxValue)
    {
        StartCoroutine(HitEffectCoroutine(_flashDuration));
    }

    private IEnumerator HitEffectCoroutine(float duration)
    {
        // Swap to hit material
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = _hitMaterial;
        }

        yield return new WaitForSeconds(duration);

        // Revert to original material
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = _originalMaterials[i];
        }
    }
}
