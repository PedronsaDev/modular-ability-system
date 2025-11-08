using System;
using UnityEngine;

public class HealEffectFactory : IEffectFactory<IDamageable>
{
    public float HealAmount = 10f;

    public IEffect<IDamageable> Create()
    {
        return new HealEffect()
        {
            HealAmount = this.HealAmount
        };
    }
}

[Serializable]
public struct HealEffect : IEffect<IDamageable>
{
    public float HealAmount;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public void Apply(GameObject caster ,IDamageable target)
    {
        target.Heal(HealAmount);
        OnCompleted?.Invoke(this);
    }

    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}
