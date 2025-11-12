using System;
using UnityEngine;

/// <summary>
/// Runtime instance of a knockback effect
/// </summary>
public struct KnockbackEffect : IEffect<IDamageable>
{
    private float _force;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public KnockbackEffect(float force)
    {
        _force = force;
        OnCompleted = null;
    }

    /// <summary>Applies an impulse force away from caster toward target.</summary>
    /// <param name="caster">Origin entity applying knockback.</param>
    /// <param name="target">Target to receive force.</param>
    public void Apply(GameObject caster, IDamageable target)
    {
        var targetTransform = (target as MonoBehaviour)?.gameObject.transform;

        if (!targetTransform)
            return;

        var dir = (targetTransform.position - caster.transform.position).normalized;

        if (targetTransform.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(dir * _force, ForceMode.Impulse);
            //Debug.Log($"{caster.name} knocks back {targetTransform.name} with force {_force}");
        }
        else
        {
            Debug.Log("Target has no Rigidbody to apply knockback.");
        }

        OnCompleted?.Invoke(this);
    }

    /// <summary>Cancel knockback effect (immediate completion).</summary>
    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}

/// <summary>
/// Factory for creating KnockbackEffect instances
/// </summary>
[Serializable]
public class KnockbackEffectFactory : IEffectFactory<IDamageable>
{
    public float Force = 10f;

    /// <summary>Create a new KnockbackEffect instance with configured force.</summary>
    public IEffect<IDamageable> CreateEffect()
    {
        return new KnockbackEffect(Force);
    }
}
