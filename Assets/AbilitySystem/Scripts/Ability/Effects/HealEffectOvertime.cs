using System;
using UnityEngine;

/// <summary>
/// Runtime instance of a heal over time effect
/// Kept as class due to complex state management with timers
/// </summary>
public class HealEffectOvertime : IEffect<IDamageable>
{
    private float _duration;
    private float _tickInterval;
    private float _healAmountPerTick;

    private IntervalTimer _timer;
    private IDamageable _currentTarget;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public HealEffectOvertime(float duration, float tickInterval, float healAmountPerTick)
    {
        _duration = duration;
        _tickInterval = tickInterval;
        _healAmountPerTick = healAmountPerTick;
    }

    /// <summary>Apply HoT to target; starts interval timer.</summary>
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
        _currentTarget?.Heal(_healAmountPerTick);
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

    /// <summary>Cancel the effect, stopping timer and cleaning up state.</summary>
    public void Cancel()
    {
        _timer?.Stop();
        Cleanup();
    }
}

/// <summary>
/// Factory for creating HealEffectOvertime instances
/// </summary>
[Serializable]
public class HealEffectOvertimeFactory : IEffectFactory<IDamageable>
{
    public float Duration = 5f;
    public float TickInterval = 1f;
    public float HealAmountPerTick = 10f;

    /// <summary>Create a new runtime HealEffectOvertime effect.</summary>
    public IEffect<IDamageable> CreateEffect()
    {
        return new HealEffectOvertime(Duration, TickInterval, HealAmountPerTick);
    }
}
