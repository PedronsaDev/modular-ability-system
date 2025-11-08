public class SelfTargeting : TargetingStrategy
{
    public override void Start(AbilityData ability, TargetingManager targetingManager)
    {
        this.Ability = ability;
        this.TargetingManager = targetingManager;

        if (targetingManager.transform.TryGetComponent<IDamageable>(out var target))
        {
            ability.Execute(targetingManager.gameObject, target);
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
