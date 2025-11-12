using System;
using UnityEngine;
public abstract class TargetingStrategy
{
    protected AbilityData Ability;
    protected TargetingManager TargetingManager;
    protected bool _isTargeting = false;
    protected GameObject _caster;

    public bool IsTargeting => _isTargeting;
    public event Action OnTargetingCompleted;

    public abstract void Start(AbilityData ability, TargetingManager targetingManager, GameObject caster);

    public virtual void Update() { }
    public virtual void Cancel() { }

    /// <summary>Invoke completion event for subscribers (e.g., ability slot cooldown).</summary>
    protected void RaiseTargetingComplete() => OnTargetingCompleted?.Invoke();
}