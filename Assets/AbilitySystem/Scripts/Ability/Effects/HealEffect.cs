using System;
using UnityEngine;

/// <summary>
/// Runtime instance of a heal effect
/// </summary>
public struct HealEffect : IEffect<IDamageable>
{
    private float _healAmount;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public HealEffect(float healAmount)
    {
        _healAmount = healAmount;
        OnCompleted = null;
    }

    public void Apply(GameObject caster, IDamageable target)
    {
        target.Heal(_healAmount);
        OnCompleted?.Invoke(this);
    }

    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}

/// <summary>
/// Factory for creating HealEffect instances
/// </summary>
[Serializable]
public class HealEffectFactory : IEffectFactory<IDamageable>
{
    public float HealAmount = 10;

    public IEffect<IDamageable> CreateEffect()
    {
        return new HealEffect(HealAmount);
    }
}

