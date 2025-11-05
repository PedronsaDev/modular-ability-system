using System;
using UnityEngine;

[Serializable]
public class DamageEffectFactory : IEffectFactory<IDamageable>
{
    public float DamageAmount = 10;

    public IEffect<IDamageable> Create()
    {
        return new DamageEffect {DamageAmount = this.DamageAmount};
    }
}

[Serializable]
public struct DamageEffect : IEffect<IDamageable>
{
    public float DamageAmount;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public void Apply(GameObject caster ,IDamageable target)
    {
        target.TakeDamage(DamageAmount);
        OnCompleted?.Invoke(this);
    }

    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}
