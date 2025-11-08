using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "ScriptableObjects/AbilityData", order = 1)]
public class AbilityData : ScriptableObject
{
    [Tooltip("The icon representing the ability in the UI")]
    public Sprite Icon;

    [Tooltip("The label/display name of the ability")]
    public string Label;

    [Tooltip("The description of the ability that will be displayed in the UI"), TextArea]
    public string Description;

    [Tooltip("The effect prefab to spawn on the target when the ability is applied.")]
    public GameObject EffectVFX;
    [Tooltip("Duration in seconds for which the effect VFX should last.")]
    public float EffectVFXDuration;

    [Tooltip("Time in seconds required to cast the ability")]
    [Range(0f, 5f)] public float CastTime = 1f;

    [Tooltip("Time in seconds before the ability can be used again")]
    public float CooldownTime = 1f;

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

    public void Execute(GameObject caster ,IDamageable target)
    {
        HandleVFX(target);

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

    public void HandleVFX(IDamageable target)
    {
        var targetMb = target as MonoBehaviour;

        if (!targetMb)
            return;

        if (EffectVFX)
        {
            var overtimeVFX = Instantiate(EffectVFX, targetMb.transform.position, Quaternion.identity, targetMb.transform);
            Destroy(overtimeVFX, EffectVFXDuration);
        }
    }
}
