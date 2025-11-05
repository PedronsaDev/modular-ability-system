using System;
using UnityEngine;

public interface IEffect<in TTarget>
{
    void Apply(GameObject caster,TTarget target);
    void Cancel();
    event Action<IEffect<TTarget>> OnCompleted;
}

public interface IEffectFactory<in TTarget>
{
    IEffect<TTarget> Create();
}

[Serializable]
public class DamageEffectFactory : IEffectFactory<IDamageable>
{
    public int DamageAmount = 10;

    public IEffect<IDamageable> Create()
    {
        return new DamageEffect {DamageAmount = this.DamageAmount};
    }
}

[Serializable]
public struct DamageEffect : IEffect<IDamageable>
{
    public int DamageAmount;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public void Apply(GameObject caster ,IDamageable target)
    {
        target.TakeDamage(DamageAmount);
        OnCompleted?.Invoke(this);
    }

    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}

[Serializable]
public class DamageOverTimeEffectFactory : IEffectFactory<IDamageable>
{
    public float Duration = 5f;
    public float TickInterval = 1f;
    public int AmountPerTick = 10;

    public IEffect<IDamageable> Create()
    {
        return new DamageEffectOvertime
        {
            Duration = this.Duration,
            TickInterval = this.TickInterval,
            AmountPerTick = this.AmountPerTick
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