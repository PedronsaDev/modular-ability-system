using System;
using UnityEngine;

[Serializable]
public class DamageEffect : IEffect<IDamageable>
{
    public float DamageAmount = 10f;

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
