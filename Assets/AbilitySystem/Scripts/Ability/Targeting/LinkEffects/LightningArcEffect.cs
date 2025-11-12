using UnityEngine;

/// <summary>
/// Example ScriptableObject-based link effect for lightning arcs.
/// Create instances via: Assets > Create > Chain Link Effects > Lightning Arc
/// </summary>
[CreateAssetMenu(fileName = "New Lightning Arc Effect", menuName = "Chain Link Effects/Lightning Arc", order = 1)]
public class LightningArcEffect : ChainLinkEffectBase
{
    [Header("Arc Visual Settings")]
    [Tooltip("Duration the arc visual lasts.")]
    public float Duration = 0.3f;

    [Tooltip("Intensity of the arc's random displacement.")]
    public float Intensity = 0.5f;

    [Tooltip("Number of segments in the arc (higher = smoother).")]
    public int Segments = 20;

    [Tooltip("Width of the arc line at the start.")]
    public float StartWidth = 0.15f;

    [Tooltip("Width of the arc line at the end.")]
    public float EndWidth = 0.05f;

    [Tooltip("Color gradient for the arc.")]
    public Gradient ColorGradient;

    [Tooltip("Animation speed for the arc (how often it regenerates).")]
    public float AnimationSpeed = 0.05f;

    public override void CreateEffect(Vector3 startPos, Vector3 endPos, Transform startTransform = null, Transform endTransform = null)
    {
        // Create arc GameObject
        GameObject arcObj = new GameObject("LightningArc_" + name);
        var arc = arcObj.AddComponent<LightningArc>();
        var lineRenderer = arcObj.GetComponent<LineRenderer>();

        // Configure LineRenderer visual settings
        if (lineRenderer)
        {
            // Set up material
            var material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.material = material;

            // Set width
            lineRenderer.startWidth = StartWidth;
            lineRenderer.endWidth = EndWidth;

            // Set color gradient
            if (ColorGradient != null && ColorGradient.colorKeys.Length > 0)
            {
                lineRenderer.colorGradient = ColorGradient;
            }
            else
            {
                // Default electric blue/white gradient
                var gradient = new Gradient();
                gradient.SetKeys(
                    new[] { new GradientColorKey(new Color(0.3f, 0.7f, 1f), 0f), new GradientColorKey(Color.white, 0.5f), new GradientColorKey(new Color(0.3f, 0.7f, 1f), 1f) },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
                );
                lineRenderer.colorGradient = gradient;
            }
        }

        // Configure arc settings
        arc.Segments = Segments;
        arc.Intensity = Intensity;
        arc.Duration = Duration;
        arc.AnimationSpeed = AnimationSpeed;

        // Initialize and start the arc
        arc.Initialize(startPos, endPos, Duration, Intensity);
    }
}

