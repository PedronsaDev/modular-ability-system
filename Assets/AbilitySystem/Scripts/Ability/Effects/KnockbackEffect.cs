using System;
using UnityEngine;

[Serializable]
public class KnockbackEffect : IEffect<IDamageable>
{
    public float Force = 5f;

    public event Action<IEffect<IDamageable>> OnCompleted;

    public void Apply(GameObject caster, IDamageable target)
    {
        var targetTransform = (target as MonoBehaviour)?.gameObject.transform;

        if (!targetTransform)
            return;

        var dir = (targetTransform.position - caster.transform.position).normalized;

        if (targetTransform.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(dir*Force, ForceMode.Impulse);
            //Debug.Log($"{caster.name} knocks back {targetTransform.name} with force {Force}");
        }
        else
        {
            Debug.Log("Target has no Rigidbody to apply knockback.");
        }

        OnCompleted?.Invoke(this);
    }

    public void Cancel()
    {
        OnCompleted?.Invoke(this);
    }
}
