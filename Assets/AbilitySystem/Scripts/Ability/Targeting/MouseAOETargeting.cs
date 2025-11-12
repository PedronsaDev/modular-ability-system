using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Area-of-Effect targeting via mouse position raycast to ground layer. Shows preview, waits for click,
/// then applies ability to all damageables within radius.
/// </summary>
[Serializable]
public class MouseAOETargeting : TargetingStrategy
{
    public GameObject AbilityEffectPrefab;
    public Vector3 EffectOffset;
    public GameObject AOEPreviewPrefab;
    public float AOERadius = 5f;
    public LayerMask GroundLayer;
    public AnimationClip _castAnimation;

    private GameObject _previewAOEInstance;

    /// <summary>Starts AOE targeting: creates preview and subscribes to click input.</summary>
    public override void Start(AbilityData ability, TargetingManager targetingManager, GameObject caster)
    {
        this.Ability = ability;
        this.TargetingManager = targetingManager;

        this.TargetingManager.SetCurrentStrategy(this);

        if (AOEPreviewPrefab)
        {
            _previewAOEInstance = UnityEngine.Object.Instantiate(AOEPreviewPrefab, Vector3.zero + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
            _previewAOEInstance.transform.localScale = new Vector3(AOERadius * 2, 0.5f, AOERadius * 2);
        }

        if (this.TargetingManager.Input)
            TargetingManager.ClickAction.performed += OnClickPerformed;

        _isTargeting = true;
    }

    /// <summary>Updates preview position under mouse cursor.</summary>
    public override void Update()
    {
        if (!_isTargeting || !_previewAOEInstance)
            return;

        _previewAOEInstance.transform.position = GetMouseWorldPosition() + new Vector3(0f, 0.1f, 0f);
    }

    Vector3 GetMouseWorldPosition()
    {
        if (!TargetingManager.Cam)
            return Vector3.zero;

        var mousePosition = Mouse.current.position.ReadValue();
        var ray = TargetingManager.Cam.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, GroundLayer))
            return hitInfo.point;

        return Vector3.zero;
    }

    /// <summary>Cleans up preview and input subscriptions then raises completion.</summary>
    public override void Cancel()
    {
        _isTargeting = false;

        if (_previewAOEInstance)
            UnityEngine.Object.Destroy(_previewAOEInstance);

        if (TargetingManager.Input)
            TargetingManager.ClickAction.performed -= OnClickPerformed;

        RaiseTargetingComplete();
        TargetingManager.ClearCurrentStrategy();
    }

    private void OnClickPerformed(InputAction.CallbackContext callbackContext)
    {
        if (_isTargeting)
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var ray = TargetingManager.Cam.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, GroundLayer))
            {
                var targets = Physics.OverlapSphere(hitInfo.point, AOERadius)
                    .Select(c => c.GetComponent<IDamageable>())
                    .OfType<IDamageable>();

                foreach (var target in targets)
                    Ability.Execute(TargetingManager.gameObject ,target);


                var effectPosition = hitInfo.point + EffectOffset;
                if (AbilityEffectPrefab)
                {
                    var effect = UnityEngine.Object.Instantiate(AbilityEffectPrefab, effectPosition, Quaternion.identity);
                    effect.transform.localScale = new Vector3(AOERadius, AOERadius, AOERadius);
                    UnityEngine.Object.Destroy(effect, Ability.EffectVFXDuration);
                }

                TargetingManager.GetComponent<PlayerAnimationController>().PlayOneShot(_castAnimation);
                Cancel();
            }
        }
    }
}