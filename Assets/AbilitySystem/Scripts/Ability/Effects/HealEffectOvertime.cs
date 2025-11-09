using System;
using UnityEngine;

[Serializable]
public class HealEffectOvertime : IEffect<IDamageable>
{
    public float Duration = 5;
    public float TickInterval = 1f;
    public float HealAmountPerTick = 10f;

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
        _currentTarget?.Heal(HealAmountPerTick);
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
