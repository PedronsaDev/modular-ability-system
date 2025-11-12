using UnityEngine;

/// <summary>
/// Example custom chain link effect that spawns a prefab between targets.
/// Great for custom VFX or unique ability visuals.
/// Create instances via: Assets > Create > Chain Link Effects > Prefab Link
/// </summary>
[CreateAssetMenu(fileName = "New Prefab Link Effect", menuName = "Chain Link Effects/Prefab Link", order = 10)]
public class PrefabLinkEffect : ChainLinkEffectBase
{
    [Header("Prefab Settings")]
    [Tooltip("Prefab to spawn as the link effect.")]
    public GameObject LinkPrefab;

    [Tooltip("Duration before destroying the spawned prefab.")]
    public float Duration = 0.5f;

    [Tooltip("Should the prefab face from start to end?")]
    public bool FaceDirection = true;

    [Tooltip("Spawn at start position (true) or midpoint (false).")]
    public bool SpawnAtStart = true;

    [Tooltip("Scale the prefab based on distance between targets.")]
    public bool ScaleWithDistance;

    [Tooltip("Base scale multiplier.")]
    public float ScaleMultiplier = 1f;

    public override void CreateEffect(Vector3 startPos, Vector3 endPos, Transform startTransform = null, Transform endTransform = null)
    {
        if (!LinkPrefab)
        {
            Debug.LogWarning($"PrefabLinkEffect '{name}': No LinkPrefab assigned!");
            return;
        }

        // Calculate spawn position
        Vector3 spawnPos = SpawnAtStart ? startPos : (startPos + endPos) / 2f;

        // Calculate rotation
        Quaternion rotation = Quaternion.identity;
        if (FaceDirection)
        {
            Vector3 direction = (endPos - startPos).normalized;
            if (direction.sqrMagnitude > 0.01f)
            {
                rotation = Quaternion.LookRotation(direction);
            }
        }

        // Spawn the prefab
        GameObject instance = Instantiate(LinkPrefab, spawnPos, rotation);

        // Scale if needed
        if (ScaleWithDistance)
        {
            float distance = Vector3.Distance(startPos, endPos);
            instance.transform.localScale *= distance * ScaleMultiplier;
        }
        else if (!Mathf.Approximately(ScaleMultiplier, 1f))
        {
            instance.transform.localScale *= ScaleMultiplier;
        }

        // Destroy after duration
        Destroy(instance, Duration);
    }
}

