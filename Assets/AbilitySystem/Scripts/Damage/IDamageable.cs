using UnityEngine;

/// <summary>
/// Contract for entities affected by abilities: can take damage, heal, and receive runtime effects.
/// </summary>
public interface IDamageable
{
    /// <summary>Apply damage.</summary>
    void TakeDamage(float damage);

    /// <summary>Heal entity.</summary>
    void Heal(float amount);

    /// <summary>Apply a runtime effect instance (registers its completion & begins execution).</summary>
    void ApplyEffect(GameObject caster, IEffect<IDamageable> runtimeEffect);
}
