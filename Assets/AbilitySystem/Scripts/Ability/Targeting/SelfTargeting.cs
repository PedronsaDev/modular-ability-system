using UnityEngine;
public class SelfTargeting : TargetingStrategy
{
    public override void Start(AbilityData ability, TargetingManager targetingManager, GameObject caster)
    {
        this.Ability = ability;
        this.TargetingManager = targetingManager;

        if (caster.transform.TryGetComponent<IDamageable>(out var target))
        {
            ability.Execute(caster, target);
        }

        RaiseTargetingComplete();
    }

    public override void Cancel()
    {
        _isTargeting = false;

        RaiseTargetingComplete();
        TargetingManager.ClearCurrentStrategy();
    }
}
