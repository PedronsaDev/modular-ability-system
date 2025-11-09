using System;
using System.Linq;
using UnityEngine;
[Serializable]
public class CasterAOETargeting : TargetingStrategy
{
    public float AOERadius = 5f;
    public LayerMask GroundLayer;

    public GameObject AbilityEffectPrefab;
    public Vector3 EffectOffset;
    public AnimationClip CastAnimation;

    private GameObject _previewAOEInstance;

    public override void Start(AbilityData ability, TargetingManager targetingManager, GameObject caster)
    {
        this.Ability = ability;
        this.TargetingManager = targetingManager;
        this._caster = caster;

        this.TargetingManager.SetCurrentStrategy(this);
        _isTargeting = true;

        var pos = this._caster.transform.position;
        var targets = Physics.OverlapSphere(pos, AOERadius)
            .Select(c => c.GetComponent<IDamageable>())
            .OfType<IDamageable>();

        var effectPosition = pos + EffectOffset;

        if (AbilityEffectPrefab)
        {
            var effect = UnityEngine.Object.Instantiate(AbilityEffectPrefab, effectPosition, Quaternion.identity);
            effect.transform.localScale = new Vector3(AOERadius, AOERadius, AOERadius);
            UnityEngine.Object.Destroy(effect, Ability.EffectVFXDuration);
        }

        foreach (var target in targets)
        {
            var targetMb = target as MonoBehaviour;
            if (targetMb && targetMb.gameObject == caster)
                continue;

            Ability.Execute(TargetingManager.gameObject, target);
        }

        Cancel();
    }

    public override void Cancel()
    {
        _isTargeting = false;

        RaiseTargetingComplete();
        TargetingManager.ClearCurrentStrategy();
    }
}
