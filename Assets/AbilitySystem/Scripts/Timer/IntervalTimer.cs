using System;
using UnityEngine;

/// <summary>
/// Countdown timer that fires an interval event repeatedly until completion.
/// </summary>
public class IntervalTimer : Timer
{
    private readonly float _interval;
    private float _nextInterval;

    /// <summary>Raised on each interval.</summary>
    public Action OnInterval = delegate { };

    /// <summary>
    /// Initializes the timer with total time and interval.
    /// </summary>
    /// <param name="totalTime">The total time for the timer.</param>
    /// <param name="intervalSeconds">The interval in seconds for the repeated event.</param>
    public IntervalTimer(float totalTime, float intervalSeconds) : base(totalTime)
    {
        _interval = intervalSeconds;
        _nextInterval = totalTime - _interval;
    }

    /// <summary>Ticks the timer, raising OnInterval when interval thresholds are crossed.</summary>
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

    /// <summary>Indicates if the timer has finished counting down.</summary>
    public override bool IsFinished => CurrentTime <= 0;
}
