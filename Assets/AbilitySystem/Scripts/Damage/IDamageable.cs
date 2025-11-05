using UnityEngine;
public interface IDamageable
{
    void TakeDamage(int damage);
    void ApplyEffect(GameObject caster, IEffect<IDamageable> runtimeEffect);
}
