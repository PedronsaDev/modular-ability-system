using System;
using UnityEngine;

public class HealOvertimeEffectFactory : IEffectFactory<IDamageable>
{
    public float Duration = 5f;
    public float TickInterval = 1f;
    public float HealAmountPerTick = 10f;

    public IEffect<IDamageable> Create()
    {
        return new HealEffectOvertime()
        {
            Duration = this.Duration,
            TickInterval = this.TickInterval,
            HealAmountPerTick = this.HealAmountPerTick
        };
    }
}

[Serializable]
public struct HealEffectOvertime : IEffect<IDamageable>
{
    public float Duration;
    public float TickInterval;
    public float HealAmountPerTick;

    private IntervalTimer _timer;
    private IDamageable _currentTarget;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public void Apply(GameObject caster, IDamageable target)
    {
        _currentTarget = target;

        _timer = new IntervalTimer(Duration, TickInterval);
        _timer.OnInterval = OnInterval;
        _timer.OnTimerStop = OnStop;
        _timer.Start();
    }

    private void OnInterval()
    {
        _currentTarget?.TakeDamage(HealAmountPerTick);
    }

    private void Cleanup()
    {
        _timer = null;
        _currentTarget = null;
        OnCompleted?.Invoke(this);
    }

    private void OnStop()
    {
        Cleanup();
    }

    public void Cancel()
    {
        _timer?.Stop();
        Cleanup();
    }
}
