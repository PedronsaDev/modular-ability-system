using System;
using UnityEngine;
using YourNamespace;

/// <summary>
/// Abstract base for lightweight timers updated centrally by <see cref="TimerManager"/>.
/// Supports start/stop, pause/resume, reset, and progress tracking without MonoBehaviour overhead.
/// </summary>
public abstract class Timer : IDisposable
{
    /// <summary>Current time remaining or elapsed, depending on timer implementation.</summary>
    public float CurrentTime { get; protected set; }
    /// <summary>True if the timer is currently running.</summary>
    public bool IsRunning { get; private set; }

    protected float InitialTime;

    /// <summary>Fractional progress of the timer, between 0 and 1.</summary>
    public float Progress => Mathf.Clamp(CurrentTime/InitialTime, 0, 1);

    /// <summary>Callback invoked when the timer starts.</summary>
    public Action OnTimerStart = delegate { };
    /// <summary>Callback invoked when the timer stops.</summary>
    public Action OnTimerStop = delegate { };

    /// <summary>
    /// Initializes the timer with a specified duration.
    /// </summary>
    /// <param name="value">The duration of the timer.</param>
    protected Timer(float value)
    {
        InitialTime = value;
    }

    /// <summary>Begin ticking this timer (registers with TimerManager if not already running).</summary>
    public void Start()
    {
        CurrentTime = InitialTime;
        if (!IsRunning)
        {
            IsRunning = true;
            TimerManager.RegisterTimer(this);
            OnTimerStart.Invoke();
        }
    }

    /// <summary>Stop ticking this timer (deregisters and invokes completion callbacks).</summary>
    public void Stop()
    {
        if (IsRunning)
        {
            IsRunning = false;
            TimerManager.DeregisterTimer(this);
            OnTimerStop.Invoke();
        }
    }

    /// <summary>Advance timer state each frame; implemented by concrete timers.</summary>
    public abstract void Tick();
    /// <summary>True when timer has finished its lifecycle.</summary>
    public abstract bool IsFinished { get; }

    /// <summary>Resume the timer if it was paused.</summary>
    public void Resume() => IsRunning = true;
    /// <summary>Pause the timer, stopping its progression.</summary>
    public void Pause() => IsRunning = false;

    /// <summary>Reset the timer to its initial state.</summary>
    public virtual void Reset() => CurrentTime = InitialTime;

    /// <summary>
    /// Reset the timer with a new duration.
    /// </summary>
    /// <param name="newTime">The new duration for the timer.</param>
    public virtual void Reset(float newTime)
    {
        InitialTime = newTime;
        Reset();
    }

    private bool _disposed;

    ~Timer()
    {
        Dispose(false);
    }

    // Call Dispose to ensure deregistration of the timer from the TimerManager
    // when the consumer is done with the timer or being destroyed
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            TimerManager.DeregisterTimer(this);
        }

        _disposed = true;
    }
}
