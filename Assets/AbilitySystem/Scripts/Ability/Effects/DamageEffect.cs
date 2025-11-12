using System;
using UnityEngine;

/// <summary>
/// Runtime instance of a damage effect
/// </summary>
public struct DamageEffect : IEffect<IDamageable>
{
    private float _damageAmount;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public DamageEffect(float damageAmount)
    {
        _damageAmount = damageAmount;
        OnCompleted = null;
    }

    public void Apply(GameObject caster, IDamageable target)
    {
        target.TakeDamage(_damageAmount);
        OnCompleted?.Invoke(this);
    }

    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}

/// <summary>
/// Factory for creating DamageEffect instances
/// </summary>
[Serializable]
public class DamageEffectFactory : IEffectFactory<IDamageable>
{
    public float DamageAmount = 10f;

    public IEffect<IDamageable> CreateEffect()
    {
        return new DamageEffect(DamageAmount);
    }
}

