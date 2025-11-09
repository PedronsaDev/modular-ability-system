using System;
using UnityEngine;

[Serializable]
public class HealEffect : IEffect<IDamageable>
{
    public float HealAmount = 10f;

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
