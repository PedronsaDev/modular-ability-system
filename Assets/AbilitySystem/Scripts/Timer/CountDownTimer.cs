using UnityEngine;

/// <summary>
/// Timer that counts down from initial value to zero; raises stop event on completion.
/// </summary>
public class CountdownTimer : Timer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CountdownTimer"/> class.
    /// </summary>
    /// <param name="value">The initial value for the timer.</param>
    public CountdownTimer(float value) : base(value) { }

    /// <summary>
    /// Tick down using Time.deltaTime; stops when reaches zero.
    /// </summary>
    public override void Tick()
    {
        if (IsRunning && CurrentTime > 0)
            CurrentTime -= Time.deltaTime;

        if (IsRunning && CurrentTime <= 0)
            Stop();
    }

    /// <summary>
    /// Gets a value indicating whether the timer has finished.
    /// </summary>
    public override bool IsFinished => CurrentTime <= 0;
}
