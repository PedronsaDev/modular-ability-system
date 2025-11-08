using UnityEngine;
public interface IDamageable
{
    void TakeDamage(float damage);
    void Heal(float amount);
    void ApplyEffect(GameObject caster, IEffect<IDamageable> runtimeEffect);
}
