using UnityEngine;

/// <summary>
/// Base ScriptableObject for chain link effects.
/// Create custom implementations for different visual effects.
/// </summary>
public abstract class ChainLinkEffectBase : ScriptableObject, IChainLinkEffect
{
    public abstract void CreateEffect(Vector3 startPos, Vector3 endPos, Transform startTransform = null, Transform endTransform = null);
}

