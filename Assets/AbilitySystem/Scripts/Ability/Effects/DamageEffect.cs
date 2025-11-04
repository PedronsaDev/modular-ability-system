using System;
using UnityEngine;
[Serializable]
public class DamageEffect : IEffect<IDamageable>
{
    public int Amount = 10;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public void Apply(GameObject caster ,IDamageable target)
    {
        target.TakeDamage(Amount);
        OnCompleted?.Invoke(this);
    }

    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}
