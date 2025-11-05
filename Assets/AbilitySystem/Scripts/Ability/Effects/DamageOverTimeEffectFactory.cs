using System;
using UnityEngine;

[Serializable]
public class DamageOverTimeEffectFactory : IEffectFactory<IDamageable>
{
    public float Duration = 5f;
    public float TickInterval = 1f;
    public int DamageAmountPerTick = 10;

    public IEffect<IDamageable> Create()
    {
        return new DamageEffectOvertime
        {
            Duration = this.Duration,
            TickInterval = this.TickInterval,
            AmountPerTick = this.DamageAmountPerTick
        };
    }
}

[Serializable]
public struct DamageEffectOvertime : IEffect<IDamageable>
{
    public float Duration;
    public float TickInterval;
    public int AmountPerTick;

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
        _currentTarget?.TakeDamage(AmountPerTick);
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
