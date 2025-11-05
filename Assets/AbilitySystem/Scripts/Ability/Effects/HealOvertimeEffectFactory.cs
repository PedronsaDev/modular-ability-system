public class HealOvertimeEffectFactory : IEffectFactory<IDamageable>
{
    public float Duration = 5f;
    public float TickInterval = 1f;
    public int HealAmountPerTick = 10;

    public IEffect<IDamageable> Create()
    {
        return new DamageEffectOvertime
        {
            Duration = this.Duration,
            TickInterval = this.TickInterval,
            AmountPerTick = -this.HealAmountPerTick
        };
    }
}
