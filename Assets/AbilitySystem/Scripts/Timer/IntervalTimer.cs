using System;
using UnityEngine;

/// <summary>
/// Countdown timer that fires an event every interval until completion.
/// </summary>
public class IntervalTimer : Timer
{
    private readonly float _interval;
    private float _nextInterval;

    public Action OnInterval = delegate { };

    public IntervalTimer(float totalTime, float intervalSeconds) : base(totalTime)
    {
        _interval = intervalSeconds;
        _nextInterval = totalTime - _interval;
    }

    public override void Tick()
    {
        if (IsRunning && CurrentTime > 0)
        {
            CurrentTime -= Time.deltaTime;

            // Fire interval events as long as thresholds are crossed
            while (CurrentTime <= _nextInterval && _nextInterval >= 0)
            {
                OnInterval.Invoke();
                _nextInterval -= _interval;
            }
        }

        if (IsRunning && CurrentTime <= 0)
        {
            CurrentTime = 0;
            Stop();
        }
    }

    public override bool IsFinished => CurrentTime <= 0;
}
