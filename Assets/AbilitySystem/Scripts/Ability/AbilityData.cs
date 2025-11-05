using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "ScriptableObjects/AbilityData", order = 1)]
public class AbilityData : ScriptableObject
{
    [Tooltip("The label/display name of the ability")]
    public string Label;

    [Tooltip("The projectile prefab to spawn when the ability is used. Leave null if no projectile is needed")]
    public Projectile VFXProjectile;

    [Tooltip("The overtime effect prefab to spawn on the target when the ability is used. Leave null if no overtime effect is needed")]
    public GameObject VFXOvertime;

    [Tooltip("Time in seconds required to cast the ability")]
    [Range(0f, 5f)] public float CastTime = 1f;

    public AnimationClip CastAnimation;

    [Header("Effects")]
    [SerializeReference] public List<IEffectFactory<IDamageable>> Effects;

    [Header("Targeting")]
    [SerializeReference] public TargetingStrategy TargetingStrategy;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(Label))
            Label = name;

        Effects ??= new List<IEffectFactory<IDamageable>>();
    }

    public void Execute(GameObject caster, IDamageable target)
    {
        HandleVFX(caster, target);

        foreach (var effect in Effects)
        {
            var runtimeEffect = effect.Create();
            target.ApplyEffect(caster, runtimeEffect);
        }
    }

    public void Target(TargetingManager targetingManager)
    {
        TargetingStrategy?.Start(this, targetingManager);
    }

    public void HandleVFX(GameObject caster, IDamageable target)
    {
        var targetMb = target as MonoBehaviour;

        if (!targetMb)
            return;

        if (VFXOvertime)
        {
            var overtimeVFX = Instantiate(VFXOvertime, targetMb.transform.position, Quaternion.identity, targetMb.transform);
            Destroy(overtimeVFX, 3f);
        }
    }
}