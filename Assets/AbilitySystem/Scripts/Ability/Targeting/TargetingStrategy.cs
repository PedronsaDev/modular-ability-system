public abstract class TargetingStrategy
{
    protected AbilityData Ability;
    protected TargetingManager TargetingManager;
    protected bool _isTargeting = false;

    public bool IsTargeting => _isTargeting;

    public abstract void Start(AbilityData ability, TargetingManager targetingManager);

    public virtual void Update() { }
    public virtual void Cancel() { }
}

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
    }
}
