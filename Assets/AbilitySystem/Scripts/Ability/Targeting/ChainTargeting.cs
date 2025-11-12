using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LinkEffectType
{
    None,
    LightningArc,
    Beam,
    ParticleTrail
}

/// <summary>
/// Sequential chain targeting: finds initial target near caster, then iteratively links to nearest
/// next targets within <see cref="ChainRadius"/> up to <see cref="MaxTargets"/>. Optional visual link effects.
/// </summary>
[Serializable]
public class ChainTargeting : TargetingStrategy
{
    [Header("Chain Settings")]
    [Tooltip("Total number of enemies to hit (max chain length).")]
    public int MaxTargets = 3;

    [Tooltip("How far from the caster to search for the very first target.")]
    public float InitialSearchRadius = 12f;

    [Tooltip("Maximum distance between chained targets.")]
    public float ChainRadius = 10f;

    [Tooltip("Delay between each chain link (seconds).")]
    public float LinkDelaySeconds = 0.15f;

    [Header("VFX")]
    [Tooltip("Optional impact VFX to spawn on each target hit.")]
    public GameObject ImpactEffectPrefab;

    public Vector3 EffectOffset;

    [Header("Link Effect Settings")]
    [Tooltip("Enable visual effects between chained targets.")]
    public bool EnableLinkEffect = true;

    [Tooltip("Type of link effect to use.")]
    public LinkEffectType EffectType = LinkEffectType.LightningArc;

    [Tooltip("Offset for the link start point (relative to source position).")]
    public Vector3 LinkStartOffset = Vector3.up;

    [Tooltip("Offset for the link end point (relative to target position).")]
    public Vector3 LinkEndOffset = Vector3.up;

    [Header("Lightning Arc Settings")]
    public LightningArcLinkEffect LightningArcSettings = new LightningArcLinkEffect();

    [Header("Beam Settings")]
    public BeamLinkEffect BeamSettings = new BeamLinkEffect();

    [Header("Particle Trail Settings")]
    public ParticleTrailLinkEffect ParticleTrailSettings = new ParticleTrailLinkEffect();

    [Header("Custom Effect")]
    [Tooltip("Optional custom link effect ScriptableObject. Overrides EffectType if set.")]
    public ChainLinkEffectBase CustomLinkEffect;

    public LayerMask TargetSearchMask = ~0; // default: everything

    private Coroutine _chainRoutine;
    private static readonly Collider[] _overlapBuffer = new Collider[64]; // tweak size as needed

    /// <summary>Starts chain routine coroutine on the TargetingManager.</summary>
    public override void Start(AbilityData ability, TargetingManager targetingManager, GameObject caster)
    {
        this.Ability = ability;
        this.TargetingManager = targetingManager;
        this._caster = caster;

        TargetingManager.SetCurrentStrategy(this);
        _isTargeting = true;

        // Execute the chain over time with delays.
        _chainRoutine = TargetingManager.StartCoroutine(ExecuteChainRoutine());
    }

    private IEnumerator ExecuteChainRoutine()
    {
        if (MaxTargets <= 0 || !_caster)
        {
            Cancel();
            yield break;
        }

        var visited = new HashSet<IDamageable>();

        // 1) Find the first target near the caster
        var current = FindNearestTarget(_caster.transform.position, InitialSearchRadius, visited);
        if (current == null)
        {
            Cancel();
            yield break;
        }

        // Track previous position and transform for link effects
        Vector3 previousPos = _caster.transform.position;
        Transform previousTransform = _caster.transform;

        // 2) Apply to first, then keep chaining with delay
        for (int i = 0; i < MaxTargets && current != null && _isTargeting; i++)
        {
            var currentMb = current as MonoBehaviour;
            if (!currentMb)
                break;

            // Create link effect from previous position to current target
            if (EnableLinkEffect)
            {
                Vector3 linkStart = previousPos + LinkStartOffset;
                Vector3 linkEnd = currentMb.transform.position + LinkEndOffset;
                CreateLinkEffect(linkStart, linkEnd, previousTransform, currentMb.transform);
            }

            ApplyToTarget(current);
            visited.Add(current);

            // Update previous position and transform for next link
            previousPos = currentMb.transform.position;
            previousTransform = currentMb.transform;

            // Delay between links (skip after the final link)
            if (i < MaxTargets - 1 && LinkDelaySeconds > 0f)
                yield return new WaitForSeconds(LinkDelaySeconds);

            // find next
            current = FindNearestTarget(currentMb.transform.position, ChainRadius, visited);
        }

        Cancel();
    }

    private IDamageable FindNearestTarget(Vector3 center, float radius, HashSet<IDamageable> exclude)
    {
        int count = Physics.OverlapSphereNonAlloc(center, radius, _overlapBuffer, TargetSearchMask);
        if (count <= 0)
            return null;

        IDamageable best = null;
        float bestDistSq = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            var c = _overlapBuffer[i];
            if (!c)
                continue;
            if (_caster && c.gameObject == _caster)
                continue;

            var candidate = c.GetComponent<IDamageable>();
            if (candidate == null)
                continue;
            if (exclude != null && exclude.Contains(candidate))
                continue;

            var mb = candidate as MonoBehaviour;
            if (!mb)
                continue;

            float dSq = (mb.transform.position - center).sqrMagnitude;
            if (dSq < bestDistSq)
            {
                bestDistSq = dSq;
                best = candidate;
            }
        }

        return best;
    }

    private void ApplyToTarget(IDamageable target)
    {
        if (target == null || !Ability)
            return;

        // Execute ability effect on target
        Ability.Execute(_caster ? _caster : TargetingManager.gameObject, target);

        // Spawn optional impact VFX
        if (ImpactEffectPrefab)
        {
            var mb = target as MonoBehaviour;
            if (mb)
            {
                var pos = mb.transform.position + EffectOffset;
                var vfx = UnityEngine.Object.Instantiate(ImpactEffectPrefab, pos, Quaternion.identity);
                if (Ability)
                    UnityEngine.Object.Destroy(vfx, Ability.EffectVFXDuration);
            }
        }
    }

    private void CreateLinkEffect(Vector3 start, Vector3 end, Transform startTransform, Transform endTransform)
    {
        if (!EnableLinkEffect)
            return;

        IChainLinkEffect effect = null;

        // Use custom effect if provided
        if (CustomLinkEffect != null)
        {
            effect = CustomLinkEffect;
        }
        else
        {
            // Otherwise use the selected effect type
            switch (EffectType)
            {
                case LinkEffectType.LightningArc:
                    effect = LightningArcSettings;
                    break;
                case LinkEffectType.Beam:
                    effect = BeamSettings;
                    break;
                case LinkEffectType.ParticleTrail:
                    effect = ParticleTrailSettings;
                    break;
                case LinkEffectType.None:
                    return;
            }
        }

        effect?.CreateEffect(start, end, startTransform, endTransform);
    }

    /// <summary>Cancels chain targeting and stops coroutine execution.</summary>
    public override void Cancel()
    {
        // Stop the running coroutine if any
        if (_chainRoutine != null)
        {
            TargetingManager.StopCoroutine(_chainRoutine);
            _chainRoutine = null;
        }

        _isTargeting = false;

        RaiseTargetingComplete();
        TargetingManager.ClearCurrentStrategy();
    }
}
