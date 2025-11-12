using UnityEngine;
/// <summary>
/// Executes the ability immediately against the caster if it implements <see cref="IDamageable"/>.
/// </summary>
public class SelfTargeting : TargetingStrategy
{
    /// <summary>Starts self targeting; applies ability instantly.</summary>
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

    /// <summary>Cancels (no-op for self) ensuring cooldown can begin.</summary>
    public override void Cancel()
    {
        _isTargeting = false;

        RaiseTargetingComplete();
        TargetingManager.ClearCurrentStrategy();
    }
}
