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

    protected void RaiseTargetingComplete() => OnTargetingCompleted?.Invoke();
}