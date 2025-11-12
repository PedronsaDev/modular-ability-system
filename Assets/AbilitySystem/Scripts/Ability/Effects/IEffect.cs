using System;
using UnityEngine;

/// <summary>
/// Represents a runtime effect applied to a target. Implementations encapsulate state (e.g. over-time ticks)
/// and must invoke <see cref="OnCompleted"/> when finished or canceled.
/// </summary>
public interface IEffect<in TTarget>
{
    /// <summary>Apply the effect to the target.</summary>
    /// <param name="caster">Originator of the effect.</param>
    /// <param name="target">Target instance.</param>
    void Apply(GameObject caster,TTarget target);
    /// <summary>Cancel the effect prematurely; should still raise completion.</summary>
    void Cancel();
    /// <summary>Raised when the effect finishes naturally or is canceled.</summary>
    event Action<IEffect<TTarget>> OnCompleted;
}