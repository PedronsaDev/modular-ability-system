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
    [SerializeReference] public List<IEffect<IDamageable>> Effects;

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(Label))
            Label = name;

        Effects ??= new List<IEffect<IDamageable>>();
    }

    public void Execute(GameObject caster, IDamageable target)
    {
        foreach (IEffect<IDamageable> effect in Effects)
        {
            if (target is Enemy enemy)
                enemy.ApplyEffect(caster, effect);
            else
                effect.Apply(caster, target);
        }
    }
}