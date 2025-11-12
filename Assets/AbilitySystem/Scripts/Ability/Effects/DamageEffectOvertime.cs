using System;
using UnityEngine;

/// <summary>
/// Runtime instance of a damage over time effect
/// Kept as class due to complex state management with timers
/// </summary>
public class DamageEffectOvertime : IEffect<IDamageable>
{
    private float _duration;
    private float _tickInterval;
    private float _damageAmountPerTick;

    private IntervalTimer _timer;
    private IDamageable _currentTarget;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public DamageEffectOvertime(float duration, float tickInterval, float damageAmountPerTick)
    {
        _duration = duration;
        _tickInterval = tickInterval;
        _damageAmountPerTick = damageAmountPerTick;
    }

    public void Apply(GameObject caster, IDamageable target)
    {
        _currentTarget = target;

        _timer = new IntervalTimer(_duration, _tickInterval);
        _timer.OnInterval = OnInterval;
        _timer.OnTimerStop = OnStop;
        _timer.Start();
    }

    private void OnInterval()
    {
        _currentTarget?.TakeDamage(_damageAmountPerTick);
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

/// <summary>
/// Factory for creating DamageEffectOvertime instances
/// </summary>
[Serializable]
public class DamageEffectOvertimeFactory : IEffectFactory<IDamageable>
{

    public float Duration = 5f;
    public float TickInterval = 5f;
    public float DamageAmountPerTick = 5f;

    public IEffect<IDamageable> CreateEffect()
    {
        return new DamageEffectOvertime(Duration, TickInterval, DamageAmountPerTick);
    }
}

