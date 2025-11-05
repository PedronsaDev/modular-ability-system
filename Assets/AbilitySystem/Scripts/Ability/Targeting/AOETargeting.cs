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

    private GameObject _previewAOEInstance;

    public override void Start(AbilityData ability, TargetingManager targetingManager)
    {
        Debug.Log("Starting AOE Targeting");

        this.Ability = ability;
        this.TargetingManager = targetingManager;
        _isTargeting = true;

        this.TargetingManager.SetCurrentStrategy(this);

        if (AOEPrefab)
            _previewAOEInstance = UnityEngine.Object.Instantiate(AOEPrefab, Vector3.zero + new Vector3(0f, 0.1f, 0f), Quaternion.identity);

        if (this.TargetingManager.Input)
        {
            Debug.Log("Registering Click Event");
            TargetingManager.ClickAction.performed += OnClickPerformed;
        }
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
        TargetingManager.ClearCurrentStrategy();

        if (_previewAOEInstance)
            UnityEngine.Object.Destroy(_previewAOEInstance);

        if (TargetingManager.Input)
            TargetingManager.ClickAction.performed -= OnClickPerformed;
    }

    private void OnClickPerformed(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("aaaaaaaaaa");
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

                Cancel();
            }
        }
    }
}
