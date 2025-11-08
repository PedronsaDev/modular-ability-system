using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class AOETargeting : TargetingStrategy
{

    public GameObject AOEPrefab;
    public float AOERadius = 5f;
    public LayerMask GroundLayer;
    public AnimationClip _castAnimation;

    private GameObject _previewAOEInstance;

    public override void Start(AbilityData ability, TargetingManager targetingManager)
    {
        this.Ability = ability;
        this.TargetingManager = targetingManager;

        this.TargetingManager.SetCurrentStrategy(this);

        if (AOEPrefab)
        {
            _previewAOEInstance = UnityEngine.Object.Instantiate(AOEPrefab, Vector3.zero + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
            _previewAOEInstance.transform.localScale = new Vector3(AOERadius * 2, 0.5f, AOERadius * 2);
        }

        if (this.TargetingManager.Input)
            TargetingManager.ClickAction.performed += OnClickPerformed;

        _isTargeting = true;
    }

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


                TargetingManager.GetComponent<PlayerAnimationController>().PlayOneShot(_castAnimation);
                Cancel();
            }
        }
    }
}
