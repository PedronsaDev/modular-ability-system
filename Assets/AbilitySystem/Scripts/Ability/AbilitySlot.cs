using System;

[Serializable]
public class AbilitySlot
{
    public AbilityData Ability { get; private set; }
    public bool CanUse { get; private set; }

    private CountdownTimer _cooldownTimer;

    public event Action OnCooldownStart;

    public event Action OnCooldownComplete;

    public event Action<AbilitySlot> OnInitialize;

    public void Initialize(AbilityData ability)
    {
        if (Ability)
        {
            Ability.TargetingStrategy.OnTargetingCompleted -= StartCooldown;
        }

        Ability = ability;
        CanUse = true;
        Ability.TargetingStrategy.OnTargetingCompleted += StartCooldown;
        OnInitialize?.Invoke(this);
    }

    private void StartCooldown()
    {
        if (Ability.CooldownTime <= 0)
            return;

        OnCooldownStart?.Invoke();

        CanUse = false;
        _cooldownTimer = new CountdownTimer(Ability.CooldownTime);
        _cooldownTimer.OnTimerStop = () =>
        {
            CanUse = true;
            OnCooldownComplete?.Invoke();
        };

        _cooldownTimer.Start();
    }
    public void SetAbility(AbilityData abilityData)
    {
        Initialize(abilityData);
    }
}
