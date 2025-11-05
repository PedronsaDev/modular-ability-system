using UnityEngine;

public class HealEffectFactory : IEffectFactory<IDamageable>
{
    public float HealAmount = 10f;

    public IEffect<IDamageable> Create()
    {
        return new DamageEffect()
        {
            DamageAmount = -HealAmount
        };
    }
}
