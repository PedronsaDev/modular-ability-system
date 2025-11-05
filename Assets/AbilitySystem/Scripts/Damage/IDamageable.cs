using UnityEngine;
public interface IDamageable
{
    void TakeDamage(float damage);
    void ApplyEffect(GameObject caster, IEffect<IDamageable> runtimeEffect);
}
