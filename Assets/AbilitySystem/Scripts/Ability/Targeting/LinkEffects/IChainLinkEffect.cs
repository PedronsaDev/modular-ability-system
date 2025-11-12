using UnityEngine;

/// <summary>
/// Interface for visual effects that appear between chained targets.
/// Implement this to create custom link effects (arcs, beams, particles, etc.)
/// </summary>
public interface IChainLinkEffect
{
    /// <summary>
    /// Creates and displays the link effect between two positions.
    /// </summary>
    /// <param name="startPos">Starting position of the link</param>
    /// <param name="endPos">Ending position of the link</param>
    /// <param name="startTransform">Optional transform of the source (can be null)</param>
    /// <param name="endTransform">Optional transform of the target (can be null)</param>
    void CreateEffect(Vector3 startPos, Vector3 endPos, Transform startTransform = null, Transform endTransform = null);
}

